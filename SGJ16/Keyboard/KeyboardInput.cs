using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16
{
    public class KeyboardInput
    {
        private KeyboardState oldKState;
        private KeyboardState newKState;

        public KeyboardInput()
        {
            oldKState = newKState = Keyboard.GetState();
        }

        public void Update()
        {
            oldKState = newKState;
            newKState = Keyboard.GetState();
        }

        public bool IsKeyPressed(Keys key)
        {
            return newKState.IsKeyDown(key);
        }

        public bool IsKeyUp(Keys key)
        {
            return oldKState.IsKeyDown(key) && newKState.IsKeyUp(key);
        }

        public bool IsKeyDown(Keys key)
        {
            return oldKState.IsKeyUp(key) && newKState.IsKeyDown(key);
        }
    }
}
