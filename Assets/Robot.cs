using NUnit.Framework.Internal.Commands;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Robot : MonoBehaviour
{
    public float delay;
    public float StatusDelay;
    public float moveSpeed = 5f;
    public float jumpHeight = 2f;
    private bool isJumping = false;
    string state = "idle";
    public enum Commands
    {
        FORWARD,
        BACKWARD,
        JUMP,
    }

    private Queue<RobotCommand> commandQueue = new Queue<RobotCommand>();
    private bool isExecuting = false;

    private Vector3 targetPosition;

    void Start()
    {
        StartCoroutine(Status());

    }

    void Update()
    {
        if (targetPosition != Vector3.zero)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            if (transform.position == targetPosition)
            {
                targetPosition = Vector3.zero;
            }
        }
    }

    public void Sendcommands(int value)
    {
        commandQueue.Enqueue(new RobotCommand(value, delay));

        if (!isExecuting)
        {
            StartCoroutine(ExecuteCommandsWithDelay());
        }
    }

    private IEnumerator ExecuteCommandsWithDelay()
    {
        isExecuting = true;
        while (commandQueue.Count > 0)
        {
            RobotCommand nextCommand = commandQueue.Dequeue();
            yield return new WaitForSeconds(nextCommand.delay);
            ExecutCommand((Commands)nextCommand.action);
        }
        state = "idle";
        isExecuting = false;
    }

    void ExecutCommand(Commands value)
    {
        switch (value)
        {
            case Commands.FORWARD:
                state = "forward";
                targetPosition = transform.position + transform.forward * 2f;
                break;
            case Commands.BACKWARD:
                state = "backward";
                targetPosition = transform.position - transform.forward * 2f;
                break;
            case Commands.JUMP:
                state = "Jumping";
                if (!isJumping)
                {
                    StartCoroutine(Jump());
                }
                break;
            default:
                break;
        }
    }
    private IEnumerator Jump()
    {
        isJumping = true;
        Vector3 originalPosition = transform.position;
        Vector3 targetJumpPosition = transform.position + Vector3.up * jumpHeight;

        float elapsedTime = 0f;
        float jumpDuration = 0.5f;

        while (elapsedTime < jumpDuration)
        {
            transform.position = Vector3.Lerp(originalPosition, targetJumpPosition, (elapsedTime / jumpDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < jumpDuration)
        {
            transform.position = Vector3.Lerp(targetJumpPosition, originalPosition, (elapsedTime / jumpDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        state = "idle";
        isJumping = false;
    }

    private IEnumerator Status()
    {
        while (true)
        {
            yield return new WaitForSeconds(StatusDelay);
                Debug.Log(state);
        }
    }
}

public struct RobotCommand
{
    public int action;
    public float delay;

    public RobotCommand(int action, float delay)
    {
        this.action = action;
        this.delay = delay;
    }
}
