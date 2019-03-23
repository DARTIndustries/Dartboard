using DartboardEngine.Utility;
using SharpDX.XInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DartboardEngine.Input
{
    public class XInputController: IUpdateable
    {
        public const float DEAD_ZONE = 0.1f;

        private readonly Controller _Controller;
        public bool IsConnected { get; private set; }

        public Gamepad State { get; private set; }
        public Gamepad PreviousState { get; private set; }

        public XInputController(byte playerNumber)
        {
            if(playerNumber > 4)
            {
                throw new ArgumentException("Player index must be less than or equal to 4");
            }

            _Controller = new Controller((UserIndex)playerNumber);
            IsConnected = _Controller.IsConnected;
            State = PreviousState = _Controller.GetState().Gamepad;

        }

        public void Update()
        {
            if (!_Controller.IsConnected)
            {
                IsConnected = false;
                return;
            }
            IsConnected = true;
            PreviousState = State;
            State = _Controller.GetState().Gamepad;
        }

        public Vector DeltaLeftStick() => State.LeftStick() - PreviousState.LeftStick();
        public Vector DeltaRightStick() => State.RightStick() - PreviousState.RightStick();
        public float DeltaLeftTrigger => (State.LeftTrigger - PreviousState.LeftTrigger) / 255f;
        public float DeltaRightTrigger => (State.RightTrigger - PreviousState.RightTrigger) / 255f;

        public bool IsDown(GamepadButtonFlags button) => State.Buttons.HasFlag(button);
        public bool IsPressed(GamepadButtonFlags button) => State.Buttons.HasFlag(button) && !PreviousState.Buttons.HasFlag(button);
        public bool IsReleased(GamepadButtonFlags button) => !State.Buttons.HasFlag(button) && PreviousState.Buttons.HasFlag(button);

        public GamepadButtonFlags PressedButtons() => State.Buttons & (State.Buttons ^ PreviousState.Buttons);
        public GamepadButtonFlags ReleasedButtons() => (~State.Buttons) & (State.Buttons ^ PreviousState.Buttons);
    }

    public static class GamepadExtensions
    {
        public static Vector LeftStick(this Gamepad gamepad)
        {
            Vector result = new Vector(gamepad.LeftThumbX / (float)short.MaxValue, gamepad.LeftThumbY / (float)short.MaxValue, 0);
            if (result.MagnitudeSquared() <= XInputController.DEAD_ZONE)
                return Vector.Zero;
            return result;
        }
        public static Vector RightStick(this Gamepad gamepad)
        {
            Vector result = new Vector(gamepad.RightThumbX / (float)short.MaxValue, gamepad.RightThumbY / (float)short.MaxValue, 0);
            if (result.MagnitudeSquared() <= XInputController.DEAD_ZONE)
                return Vector.Zero;
            return result;
        }
    }
}
