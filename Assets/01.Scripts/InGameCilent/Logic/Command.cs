using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Command는 게임 로직에서 즉시 일어나는 모든 사건을 수집하여
// 게임의 시각적 부분에서 일정한 순서로 점진적으로 보여주는 데 사용됩니다.
public abstract class Command
{
    // 현재 게임의 시각적 부분에서 어떤 명령이 실행 중이면 true,
    // 명령 대기열이 비어 있으면 false로 설정됩니다.
    public static bool playingQueue { get; set; }

    // 명령의 컬렉션 (선입선출 방식의 대기열)
    static Queue<Command> CommandQueue = new Queue<Command>();

    // 새 명령이 생성될 때마다 항상 호출되어야 하는 메서드.
    // 예시: 
    // new DelayCommand(3f).AddToQueue(); - 3초의 지연을 CommandQueue에 추가합니다.
    public void AddToQueue()
    {
        CommandQueue.Enqueue(this);
        if (!playingQueue)
            PlayFirstCommandFromQueue();
    }

    // 이 메서드 안에 이 명령으로 하고 싶은 모든 작업을 포함시켜야 합니다
    // (카드를 뽑기, 카드를 플레이하기, 주문 효과 실행 등...)
    // StartCommandExecution에서 시각적 부분의 메서드들만 호출해야 합니다.
    // 두 가지 타이밍 옵션이 있습니다:
    // 1) Tween 시퀀스를 사용하고 OnComplete()에서 CommandExecutionComplete를 호출
    // 2) 코루틴(IEnumerator)과 WaitFor...을 사용하여 지연을 도입하고, 코루틴 끝에서 CommandExecutionComplete()를 호출
    public abstract void StartCommandExecution();

    // 명령 대기열에 카드 뽑기 명령이 있는지 확인하는 메서드
    public static bool CardDrawPending()
    {
        foreach (Command c in CommandQueue)
        {
            if (c is DrawACardCommand)
                return true;
        }
        return false;
    }

    // CommandQueue에서 다음 명령으로 이동하는 메서드
    public static void CommandExecutionComplete()
    {
        if (CommandQueue.Count > 0)
            PlayFirstCommandFromQueue();
        else
            playingQueue = false;
    }

    // CommandQueue의 첫 번째 명령을 실행하는 메서드
    static void PlayFirstCommandFromQueue()
    {
        playingQueue = true;
        CommandQueue.Dequeue().StartCommandExecution();
    }

    // 씬이 다시 로드될 때 호출되는 메서드
    public static void OnSceneReload()
    {
        CommandQueue.Clear(); // 대기열을 비움
        CommandExecutionComplete(); // 명령 실행 완료 호출
    }
}
