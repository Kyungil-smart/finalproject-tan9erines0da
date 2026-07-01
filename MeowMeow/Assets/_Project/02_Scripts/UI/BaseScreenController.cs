using UnityEngine;
using UnityEngine.UIElements;

public abstract class BaseScreenController : MonoBehaviour
{
    // 현재 화면에 활성화되어 있는 메인 판넬 변수
    protected UIPanel CurrentActivePanel;

    /// <summary>
    /// 외부 진입 버튼을 통해 특정 판넬을 지정하여 
    /// 오픈하고자 할 때 데이터를 보관하는 프로퍼티입니다.
    /// </summary>
    public UIPanel SpecificTargetPanel { get; set; }

    // 효과음
    // 처음 냥스타그램 켰을때의 사운드 재생을 막기위한 마커
    private bool _isFirstOnCanvas = true;

    /// <summary>
    /// [템플릿 메서드] 콘텐츠 UI가 최초로 오픈될 때의 
    /// 연타 잠금 및 데이터 세팅의 절대적인 순서를 제어합니다.
    /// </summary>
    public void OpenController()
    {
        // 단계 1: 글로벌 터치 입력 잠금 (애니메이션 중 연타 방지)
        SetGlobalInputBlock(true);

        OnBeforeOpen();

        // 단계 2: 자식 클래스가 구현한 구체적인 첫 오픈 알맹이 실행
        ExecuteFirstOpen();

        // 단계 3: 오픈 완료 직후 데이터 동기화 훅 호출
        OnRefreshUIData();

        OnAfterOpen();

        // 단계 4: 연출이 완전히 끝나는 타이밍에 터치 잠금 해제
        SetGlobalInputBlock(false);
        SpecificTargetPanel = null;
    }

    /// <summary>
    /// 인스펙터 버튼(OnClick)에 다이렉트로 연결하여 
    /// 특정 판넬을 주입하며 오픈할 때 사용하는 중계 함수입니다.
    /// </summary>
    /// <param name="targetPanel">처음으로 켜고 싶은 특정 UIPanel</param>
    public void OpenWithTargetPanel(UIPanel targetPanel)
    {
        SpecificTargetPanel = targetPanel;
        OpenController();
    }

    /// <summary>
    /// [템플릿 메서드] 콘텐츠 UI가 완전히 종료되어 닫힐 때의 
    /// 안전한 흐름 제어 순서를 통제합니다.
    /// </summary>
    public void CloseController()
    {
        // 효과음 관련 마커
        _isFirstOnCanvas = true;

        // 효과음
        SoundManager.Instance.Invoke(AudioType.SFX_Pop_Bubble_Single_1);

        SetGlobalInputBlock(true);
        OnBeforeClose();

        // 자식 클래스가 전담하는 화면 파괴/인액티브 연산 실행
        ExecuteClose();

        OnAfterClose();
        SetGlobalInputBlock(false);
    }

    /// <summary>
    /// [템플릿 메서드] 버튼 입력 등으로 화면이 수평/수직 전환될 때 
    /// 터치 차단 및 데이터 리프레시의 순서를 통제합니다.
    /// </summary>
    /// <param name="targetPanel">새로 교체하고자 하는 타겟 판넬</param>
    public void RequestScreenChange(UIPanel targetPanel)
    {
        // 효과음
        if (!_isFirstOnCanvas) SoundManager.Instance.Invoke(AudioType.SFX_Pop_Bubble_Single_1);

        // 효과음 관련 마커
        _isFirstOnCanvas = false;

        SubscribeManager.instance.Publish<string>(SubscribeType.Log_Write, $"{targetPanel} 전환 요청");
        if (targetPanel == null || targetPanel == CurrentActivePanel)
        {
            return;
        }
        if (CurrentActivePanel != null && CurrentActivePanel.IsTransitioning)
        {
            return;
        }
        if (targetPanel.IsTransitioning)
        {
            return;
        }

        SetGlobalInputBlock(true);
        OnBeforeScreenChange(targetPanel);

        // 자식 클래스가 전담하는 1, 2깊이 정밀 분기 스왑 연산 실행
        ExecuteScreenSwap(targetPanel);

        // 전환이 이루어진 직후 새 화면에 맞는 데이터 새로고침
        OnRefreshUIData();

        OnAfterScreenChange(targetPanel);
        SetGlobalInputBlock(false);
    }

    // --- 자식 클래스가 콘텐츠 기획에 맞게 반드시 채워야 하는 핵심 메서드 ---
    protected abstract void ExecuteFirstOpen();
    protected abstract void ExecuteClose();
    protected abstract void ExecuteScreenSwap(UIPanel targetPanel);

    // --- 데이터 레이어 연동을 위해 자식이 재정의할 선택적 훅 ---
    /// <summary>
    /// 화면이 전환되거나 처음 열릴 때, 중재자(Presenter)로부터 
    /// 최신 비즈니스 데이터를 받아와 UI 요소를 갱신하는 훅 함수입니다.
    /// </summary>
    protected virtual void OnRefreshUIData() { }

    // --- 자식의 시각 연출 타이밍 확장을 위한 가상 훅 메서드 ---
    protected virtual void OnBeforeOpen() { }
    protected virtual void OnAfterOpen() { }
    protected virtual void OnBeforeClose() { }
    protected virtual void OnAfterClose() { }
    protected virtual void OnBeforeScreenChange(UIPanel target) { }
    protected virtual void OnAfterScreenChange(UIPanel target) { }

    // 애니메이션이 재생되는 동안 모든 마우스/터치 입력을 차단하는 내부 함수
    private void SetGlobalInputBlock(bool isBlock)
    {
        // TODO: 글로벌 터치 블로킹 패널 UI 컴포넌트 On/Off 로직 위치
    }
}
