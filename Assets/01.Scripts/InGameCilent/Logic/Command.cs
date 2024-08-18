using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Command�� ���� �������� ��� �Ͼ�� ��� ����� �����Ͽ�
// ������ �ð��� �κп��� ������ ������ ���������� �����ִ� �� ���˴ϴ�.
public abstract class Command
{
    // ���� ������ �ð��� �κп��� � ����� ���� ���̸� true,
    // ��� ��⿭�� ��� ������ false�� �����˴ϴ�.
    public static bool playingQueue { get; set; }

    // ����� �÷��� (���Լ��� ����� ��⿭)
    static Queue<Command> CommandQueue = new Queue<Command>();

    // �� ����� ������ ������ �׻� ȣ��Ǿ�� �ϴ� �޼���.
    // ����: 
    // new DelayCommand(3f).AddToQueue(); - 3���� ������ CommandQueue�� �߰��մϴ�.
    public void AddToQueue()
    {
        CommandQueue.Enqueue(this);
        if (!playingQueue)
            PlayFirstCommandFromQueue();
    }

    // �� �޼��� �ȿ� �� ������� �ϰ� ���� ��� �۾��� ���Խ��Ѿ� �մϴ�
    // (ī�带 �̱�, ī�带 �÷����ϱ�, �ֹ� ȿ�� ���� ��...)
    // StartCommandExecution���� �ð��� �κ��� �޼���鸸 ȣ���ؾ� �մϴ�.
    // �� ���� Ÿ�̹� �ɼ��� �ֽ��ϴ�:
    // 1) Tween �������� ����ϰ� OnComplete()���� CommandExecutionComplete�� ȣ��
    // 2) �ڷ�ƾ(IEnumerator)�� WaitFor...�� ����Ͽ� ������ �����ϰ�, �ڷ�ƾ ������ CommandExecutionComplete()�� ȣ��
    public abstract void StartCommandExecution();

    // ��� ��⿭�� ī�� �̱� ����� �ִ��� Ȯ���ϴ� �޼���
    public static bool CardDrawPending()
    {
        foreach (Command c in CommandQueue)
        {
            if (c is DrawACardCommand)
                return true;
        }
        return false;
    }

    // CommandQueue���� ���� ������� �̵��ϴ� �޼���
    public static void CommandExecutionComplete()
    {
        if (CommandQueue.Count > 0)
            PlayFirstCommandFromQueue();
        else
            playingQueue = false;
    }

    // CommandQueue�� ù ��° ����� �����ϴ� �޼���
    static void PlayFirstCommandFromQueue()
    {
        playingQueue = true;
        CommandQueue.Dequeue().StartCommandExecution();
    }

    // ���� �ٽ� �ε�� �� ȣ��Ǵ� �޼���
    public static void OnSceneReload()
    {
        CommandQueue.Clear(); // ��⿭�� ���
        CommandExecutionComplete(); // ��� ���� �Ϸ� ȣ��
    }
}
