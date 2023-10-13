using System.Collections.Concurrent;

namespace SvetilkaBot.Services
{
    public enum UserState
    {
        NoState, StartingMenu, ASCII
    }

    public class StateService
    {
        private readonly ConcurrentDictionary<long, UserState> _userMenuState = new();
        private readonly ConcurrentDictionary<long, string> _userRGBstate = new();

        public void SetMenuState(long chatID, UserState state)
        {
            _userMenuState[chatID] = state;
        }

        public void SetRGBState(long chatID, string state)
        {
            _userRGBstate[chatID] = state;
        }

        public UserState GetMenuState(long chatID)
        {
            if (_userMenuState.TryGetValue(chatID, out var state))
            {
                return state;
            }
            else
            {
                return UserState.NoState;
            }
        }

        public string GetRGBState(long chatID)
        {
            if (_userRGBstate.TryGetValue(chatID, out var state))
            {
                return state;
            }
            else
            {
                return "OFF";
            }
        }
    }
}
