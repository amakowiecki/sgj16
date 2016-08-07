using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public class PlayerInput
    {
        private Dictionary<GameKey, Keys> mapping;

        public PlayerInput()
        {
            mapping = new Dictionary<GameKey, Keys>();
            mapping.Add(GameKey.MoveLeft, Keys.Space);
            mapping.Add(GameKey.MoveRight, Keys.Space);
            mapping.Add(GameKey.LookUp, Keys.Space);
            mapping.Add(GameKey.LookDown, Keys.Space);
            mapping.Add(GameKey.Jump, Keys.Space);
            mapping.Add(GameKey.Shot, Keys.Space);
            mapping.Add(GameKey.Pause, Keys.Space);
            mapping.Add(GameKey.Quit, Keys.Space);
            mapping.Add(GameKey.Skip, Keys.Multiply);
        }

        public Keys GetKey(GameKey key)
        {
            if (mapping.ContainsKey(key))
            {
                return mapping[key];
            }
            else
            {
                throw new Exception("Nie zarejestrowano klawisza.");
            }
        }

        public void SetKey(GameKey key, Keys value)
        {
            mapping[key] = value;
        }
    }
}
