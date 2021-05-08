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
    internal class UoMarsButtonControl : Control
    {
        private readonly Texture2D _textureNormal;
        private readonly Texture2D _textureOver;
        private Texture2D _texture;
        private readonly int ButtonId;

        public UoMarsButtonControl(List<string> inputs)
        {
            // inputs[0] = UoMarsButtonControl
            // inputs[1] = x coord in gump
            // inputs[2] = y coord in gump
            // inputs[3] = button id
            // inputs[4] = normal image name
            // inputs[5] = over/pressed image name

            try
            {
                var uoMarsPicControlBytesNormal = Convert.FromBase64String(File.ReadAllText(Path.Combine(Settings.GlobalSettings.UltimaOnlineDirectory, $"Gumps/{inputs[4]}.uomgpic")));
                _textureNormal = Texture2D.FromStream(Client.Game.GraphicsDevice, new MemoryStream(uoMarsPicControlBytesNormal));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Missing 'Gumps/{inputs[4]}.uomgpic' resource!");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return;
            }
            
            
            try 
            {
                var uoMarsPicControlBytesOver = Convert.FromBase64String(
                    File.ReadAllText(
                        Path.Combine(Settings.GlobalSettings.UltimaOnlineDirectory, $"Gumps/{inputs[5]}.uomgpic")    
                    )
                );
                _textureOver = Texture2D.FromStream(Client.Game.GraphicsDevice, new MemoryStream(uoMarsPicControlBytesOver));
            }
            catch (Exception e)
            {
                Console.WriteLine($"Missing 'Gumps/{inputs[5]}.uomgpic' resource!");
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
                return;
            }
            
            _texture = _textureNormal;
            Width = _textureNormal.Width;
            Height = _textureNormal.Height;
            
            X = Convert.ToInt32(inputs[1]);
            Y = Convert.ToInt32(inputs[2]);
            ButtonId = Convert.ToInt32(inputs[3]);
            WantUpdateSize = false;
            
            WantUpdateSize = false;
            IsFromServer = true;
            CanMove = false;
            AcceptMouseInput = true;
            CanCloseWithEsc = false;
        }
        
        public override ClickPriority Priority => ClickPriority.High;
        
        protected override void OnMouseEnter(int x, int y)
        {
            _texture = _textureOver;
            Width = _texture.Width;
            Height = _texture.Height;
        }

        protected override void OnMouseExit(int x, int y)
        {
            _texture = _textureNormal;
            Width = _texture.Width;
            Height = _texture.Height;
        }
        
        protected override void OnMouseUp(int x, int y, MouseButtonType button)
        {
            if (button == MouseButtonType.Left)
            {
                if (Client.Game.Scene is GameScene)
                {
                    OnButtonClick(ButtonId);

                    Mouse.LastLeftButtonClickTime = 0;
                    Mouse.CancelDoubleClick = true;
                }
            }
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