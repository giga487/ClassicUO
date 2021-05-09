using System;
using System.Collections.Generic;
using System.IO;
using ClassicUO.Configuration;
using ClassicUO.Game.Scenes;
using ClassicUO.Input;
using ClassicUO.Renderer;
using Microsoft.Xna.Framework.Graphics;

namespace ClassicUO.Game.UI.Controls
{
    internal class UoMarsPicControl : Control
    {
        private readonly Texture2D _texture;

        public UoMarsPicControl(List<string> inputs)
        {
            CanMove = true;
            AcceptMouseInput = true;
            
            // inputs[0] = UoMarsPicControl
            // inputs[1] = x coord in gump
            // inputs[2] = y coord in gump
            // inputs[3] = image name
            try
            {
                var uoMarsPicControlBytes = Convert.FromBase64String(File.ReadAllText(Path.Combine(Settings.GlobalSettings.UltimaOnlineDirectory, $"Gumps/{inputs[3]}.uomgpic")));
                _texture = Texture2D.FromStream(Client.Game.GraphicsDevice, new MemoryStream(uoMarsPicControlBytes));
            }
            catch (Exception e)
            {
                Console.WriteLine();
                string path = Path.Combine(Settings.GlobalSettings.UltimaOnlineDirectory, $"Gumps/{inputs[3]}.uomgpic");
                Console.WriteLine($"Missing '{path}' resource!");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return;
            }

            X = Convert.ToInt32(inputs[1]);
            Y = Convert.ToInt32(inputs[2]);
            Width = _texture.Width;
            Height = _texture.Height;
            WantUpdateSize = false;
        }

        public override bool Draw(UltimaBatcher2D batcher, int x, int y)
        {
            ResetHueVector();
            
            batcher.Draw2D
            (
                _texture,
                x,
                y,
                Width,
                Height,
                ref HueVector
            );
            
            return base.Draw(batcher, x, y);
        }
    }
}