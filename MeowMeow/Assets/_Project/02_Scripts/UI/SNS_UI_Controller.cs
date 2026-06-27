using System;
using System.Collections.Generic;
using UnityEngine;

public class SNS_UI_Controller : BaseScreenController
{
    [Header("시각적 틀 오브젝트 (배경, 상단바 등)")]
    [SerializeField]
    private GameObject _uiFrameObject;

    [Header("진입 시 기본으로 켤 1깊이 판넬")]
    [SerializeField]
    private UIPanel _defaultDepth1Panel;

    [Header("이전 화면 팝업 참조")]
    [SerializeField] GameObject _editCancelPopup;

    // 2깊이에서 원래 화면으로 빠져나올 때 돌아갈 1깊이 백업 변수
    private UIPanel _lastActiveDepth1Panel;

    // 편집 패널 추적용 스택
    private Stack<UIHistorySnapshot> _uiHistory = new Stack<UIHistorySnapshot>();
    // OpenController()가 실행되자마자 가장 먼저 호출됨
    protected override void OnBeforeOpen()
    {
        // 첫 패널이 켜지기 전에 시각적 틀을 먼저 활성화
        if (_uiFrameObject != null)
        {
            _uiFrameObject.SetActive(true);
        }
    }
    protected override void ExecuteFirstOpen()
    {
        // 외부에 의해 특정 지정 타겟(SpecificTargetPanel)이 주입되었는지 검사
        if (SpecificTargetPanel != null)
        {
            CurrentActivePanel = SpecificTargetPanel;

            // 주입된 타겟이 하필 2깊이 레이어라면 배경용으로 기본 1깊이도 세팅
            if (SpecificTargetPanel.PanelDepth == UIPanel.UIDepth.Depth2)
            {
                _lastActiveDepth1Panel = _defaultDepth1Panel;

                // 배경 1깊이는 연출 없이 즉시 활성화 상태로 깔아둠
                _lastActiveDepth1Panel.gameObject.SetActive(true);
            }
        }
        else
        {
            // 주입된 특수 타겟이 없다면 정석대로 기본 1깊이 판넬을 지정
            CurrentActivePanel = _defaultDepth1Panel;
        }

        // 1깊이가 최초 저장되는 타이밍 동기화
        if (CurrentActivePanel.PanelDepth == UIPanel.UIDepth.Depth1)
        {
            _lastActiveDepth1Panel = CurrentActivePanel;
        }

        // 결정된 첫 메인 화면을 진입 애니메이션과 함께 오픈
        CurrentActivePanel.OpenWithEnterAnimation(null);
    }

    protected override void ExecuteClose()
    {
        // 현재 클릭된 UI 오브젝트 추적
        var clickedObj = UnityEngine.EventSystems.EventSystem
            .current.currentSelectedGameObject;

        // DTO 초기화
        SubscribeManager.instance.Publish<SNSPostDTO>(SubscribeType.Update_PostModelData, SNSPostDTO.CreateEmpty());

        // 현재 열려있는 화면을 끄면서 퇴장 연출 재생
        if (CurrentActivePanel != null)
        {
            var clearable = CurrentActivePanel.GetComponent<ISNSPanelClearable>();

            if(clearable != null)
            {
                clearable.ClearPanelContext();
            }

            CurrentActivePanel.Close(null);
        }

        // 배경에 숨어서 Active 상태로 깔려있던 1깊이 화면도 함께 정리
        if (_lastActiveDepth1Panel != null &&
            _lastActiveDepth1Panel != CurrentActivePanel)
        {
            _lastActiveDepth1Panel.gameObject.SetActive(false);
        }
    }

    // CloseController()가 실행되어 패널들을 끄기 직전에 호출됨
    protected override void OnBeforeClose()
    {
        // 패널들이 완전히 꺼지거나 닫히는 흐름에 맞춰 틀도 함께 비활성화
        if (_uiFrameObject != null)
        {
            _uiFrameObject.SetActive(false);
        }
    }

    protected override void ExecuteScreenSwap(UIPanel targetPanel)
    {
        // 1깊이 수평 이동일 때는 히스토리를 초기화합니다.
        if (targetPanel.PanelDepth == UIPanel.UIDepth.Depth1)
        {
            _uiHistory.Clear();
            ProcessNavigateToDepth1(targetPanel);
        }
        else
        {
            // 2깊이로 새로 진입하거나 전진할 때만 
            // 현재 패널의 데이터를 '확정 세이브(Submit)'하고 스택에 넣습니다.
            ISNSPanelPresenter oldPresenter =
                CurrentActivePanel.GetComponent<ISNSPanelPresenter>();
            if (oldPresenter != null)
            {
                oldPresenter.SubmitContext();
            }

            SubscribeManager.instance.Publish<Action<SNSPostDTO>>(
                SubscribeType.Request_CurrentPostContext, (pureMasterData) =>
                {
                    UIHistorySnapshot history = new UIHistorySnapshot
                    {
                        Panel = CurrentActivePanel,
                        DataContext = pureMasterData 
                    };
                    _uiHistory.Push(history);
                });

            ProcessNavigateToDepth2(targetPanel);
        }
    }

    /// <summary>
    /// 2깊이 편집 단계에서 '이전 단계로' 버튼을 눌렀을 때 
    /// 데이터를 저장하지 않고 완벽하게 직전 단계로 롤백하는 후진 함수
    /// </summary>
    public void ClickNavigateBackDepth2Button()
    {
        _editCancelPopup.SetActive(true);
        // 스티커 터치 제어를 위한 코드
        TouchInputHandler.Instance.OnEditCancel = true;
    }

    /// <summary>
    /// 2깊이 편집 단계에서 '이전 단계로' 버튼을 눌렀을 때 
    /// 데이터를 저장하지 않고 완벽하게 직전 단계로 롤백하는 후진 함수
    /// </summary>
    public void ClickBackDepth2Button()
    {
        Debug.Log("클릭 성공");

        // 팝업을 닫는다.
        _editCancelPopup.SetActive(false);
        // 스티커 터치 제어를 위한 코드
        if (TouchInputHandler.Instance.OnEditCancel == true)
        {
            TouchInputHandler.Instance.OnEditCancel = false;
        }

        // 스택에 돌아갈 기록이 남아있는지 검사
        if (_uiHistory == null || _uiHistory.Count == 0)
        {
            // 더 이상 돌아갈 2깊이가 없다면 최초 1깊이 피드로 탈출
            ClickCloseDepth2Button();
            return;
        }

        if (CurrentActivePanel == null || CurrentActivePanel.IsTransitioning) return;

        // 현재 패널의 SubmitContext()를 호출하지 않고 화면을 닫음으로써
        // 이번 단계에서 유저가 수정한 구조체 스냅샷 데이터는 공중분해됩니다.
        CurrentActivePanel.Close(() =>
        {
            // 스택에서 직전 화면 기록과 '당시의 순수 데이터'를 팝합니다.
            UIHistorySnapshot previousHistory = _uiHistory.Pop();
            CurrentActivePanel = previousHistory.Panel;

            if (CurrentActivePanel.PanelDepth == UIPanel.UIDepth.Depth1)
            {
                _lastActiveDepth1Panel = CurrentActivePanel;
            }

            SubscribeManager.instance.Publish<SNSPostDTO>(
                SubscribeType.Update_PostModelData, previousHistory.DataContext);

            // 원복된 데이터를 기반으로 컴포넌트 눈금 재배치 호출
            RefreshPanelData(CurrentActivePanel);

            CurrentActivePanel.OpenWithTabChangeAnimation(null);
        });
    }

    // 1깊이 타겟으로의 화면 전환 연산을 전담하는 내부 함수
    private void ProcessNavigateToDepth1(UIPanel targetPanel)
    {
        // 기존에 보던 화면이 2깊이 장막이었던 경우
        if (CurrentActivePanel.PanelDepth == UIPanel.UIDepth.Depth2)
        {
            CurrentActivePanel.Close(() =>
            {
                // 배경에 깔려있던 이전 1깊이를 끄고 새로운 1깊이를 수평 오픈
                if (_lastActiveDepth1Panel != null)
                {
                    _lastActiveDepth1Panel.gameObject.SetActive(false);
                }

                _lastActiveDepth1Panel = targetPanel;
                CurrentActivePanel = targetPanel;
                CurrentActivePanel.OpenWithTabChangeAnimation(null);
            });
        }
        else
        {
            // 일반적인 1깊이 수평 탭 전환 (기존 거 끄고 새 거 켜기)
            CurrentActivePanel.Close(() =>
            {
                _lastActiveDepth1Panel = targetPanel;
                CurrentActivePanel = targetPanel;
                CurrentActivePanel.OpenWithTabChangeAnimation(null);
            });
        }
    }

    // 2깊이 타겟으로의 화면 전환 연산을 전담하는 내부 함수
    private void ProcessNavigateToDepth2(UIPanel targetPanel)
    {
        // 1깊이를 보다가 2깊이로 처음 진입하는 핵심 분기 상황
        if (CurrentActivePanel.PanelDepth == UIPanel.UIDepth.Depth1)
        {
            _lastActiveDepth1Panel = CurrentActivePanel;

            // 1깊이를 닫고, 2깊이 화면을 실행
            _lastActiveDepth1Panel.Close(() =>
            {
                CurrentActivePanel = targetPanel;
                RefreshPanelData(CurrentActivePanel);
                CurrentActivePanel.OpenWithEnterAnimation(null);
            });
        }
        // 이미 2깊이를 보던 중 다른 2깊이 탭으로 이동하는 상황 (4번 ➔ 5번)
        else
        {
            CurrentActivePanel.Close(() =>
            {
                CurrentActivePanel = targetPanel;
                RefreshPanelData(CurrentActivePanel);
                CurrentActivePanel.OpenWithTabChangeAnimation(null);
            });
        }
    }

    /// <summary>
    /// 2깊이 화면 X(닫기) 버튼과 연동되어 
    /// 배경에 숨어있던 직전 1깊이 화면으로 되돌아가는 독립적인 함수입니다.
    /// </summary>
    public void ClickCloseDepth2Button()
    {
        // 현재 화면이 2깊이가 맞고 돌아갈 배경 1깊이가 안전하게 존재할 때만 구동
        if (CurrentActivePanel != null &&
            CurrentActivePanel.PanelDepth == UIPanel.UIDepth.Depth2 &&
            _lastActiveDepth1Panel != null)
        {
            var clearable = CurrentActivePanel.GetComponent<ISNSPanelClearable>();
            if (clearable != null)
            {
                clearable.ClearPanelContext();
            }

            // 2깊이 장막을 걷어내는 무조건적인 클로즈 연출 실행
            CurrentActivePanel.Close(() =>
            {
                // 숨겨져 있던 직전 1깊이를 다시 현재 활성 판넬로 지정
                CurrentActivePanel = _lastActiveDepth1Panel;

                // 1깊이 화면을 다시 열어줌
                CurrentActivePanel.OpenWithTabChangeAnimation(null);
            });
        }
    }

    /// <summary>
    /// 타겟 패널의 인터페이스를 탐색하여 안전하게 데이터를 동기화하는 함수
    /// </summary>
    private void RefreshPanelData(UIPanel panel)
    {
        if (panel == null) return;

        var newPresenter = panel.GetComponent<ISNSContextReceiver>();
        if (newPresenter != null)
        {
            newPresenter.RequestContext();
        }
    }
}
public struct UIHistorySnapshot
{
    public UIPanel Panel;         // 돌아갈 직전 패널
    public SNSPostDTO DataContext; // 그 직전 패널 시절의 순수 데이터 원본
}
