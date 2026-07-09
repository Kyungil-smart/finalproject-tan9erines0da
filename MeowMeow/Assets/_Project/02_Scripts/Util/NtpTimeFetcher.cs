using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class NtpTimeFetcher
{
    private const string NtpServer = "time.google.com";
    private const int NtpPort = 123; // NTP 서버 표준 포트
    private const int TimeoutMilliseconds = 3000; // 3초 타임아웃 설정

    /// <summary>
    /// NTP 서버(구글)로부터 현재 세계 표준시(UTC)를 비동기로 가져오는 함수입니다.
    /// </summary>
    public static async Task<DateTime> GetNetworkTimeAsync(CancellationToken token)
    {
        return await Task.Run(() =>
        {
            byte[] ntpData = new byte[48];
            ntpData[0] = 0x1B; // NTP 설정 바이트 (LI = 0, VN = 3, Mode = 3)

            // 작업을 시작하기 전에 이미 취소 요청이 들어왔는지 검증합니다.
            token.ThrowIfCancellationRequested();

            try
            {
                // 도메인으로 IP 주소 획득
                IPAddress[] addresses = Dns.GetHostAddresses(NtpServer);
                IPEndPoint ipEndPoint = new IPEndPoint(addresses[0], NtpPort);

                // UDP 소켓 생성
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
                {
                    // 소켓 자체 타임아웃 세팅 (방화벽 등으로 막혔을 때 무한 대기 방지)
                    socket.SendTimeout = TimeoutMilliseconds;
                    socket.ReceiveTimeout = TimeoutMilliseconds;

                    socket.Connect(ipEndPoint);

                    // 보내기 직전 체크
                    token.ThrowIfCancellationRequested();
                    socket.Send(ntpData);

                    // 받기 직전 체크
                    token.ThrowIfCancellationRequested();
                    socket.Receive(ntpData);
                }

                // 응답 패킷에서 시간 데이터 추출 연산
                ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | ntpData[43];
                ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | ntpData[47];

                double milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

                // UTC 시간 계산
                DateTime networkDateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(milliseconds);

                // 한국 시간(KST)으로 변경하여 반환
                return networkDateTime.ToLocalTime();
            }
            catch (SocketException ex)
            {
                // 포트 막힘, 와이파이 단절 등의 경우 예외를 상위(TimeManager)로 던집니다.
                throw new Exception($"NTP 소켓 통신 실패 (포트 막힘 가능성): {ex.Message}");
            }
        }, token); 
    }
}
