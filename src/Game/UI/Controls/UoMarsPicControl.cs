using ClassicUO.Renderer;
using Microsoft.Xna.Framework.Graphics;

namespace ClassicUO.Game.UI.Controls
{
    internal class UoMarsPicControl : Control
    {
        private readonly Texture2D _texture;
        private readonly bool _tiled;

        public UoMarsPicControl(int x, int y, Texture2D texture, bool tiled = false)
        {
            CanMove = true;
            AcceptMouseInput = true;

            _texture = texture;
            X = x;
            Y = y;
            Width = texture.Width;
            Height = texture.Height;
            WantUpdateSize = false;
            _tiled = tiled;
        }

        public override bool Draw(UltimaBatcher2D batcher, int x, int y)
        {
            ResetHueVector();

            if (_tiled)
            {
                batcher.Draw2DTiled
                (
                    _texture,
                    x,
                    y,
                    Width,
                    Height,
                    ref HueVector
                );
            }
            else
            {
                batcher.Draw2D
                (
                    _texture,
                    x,
                    y,
                    Width,
                    Height,
                    ref HueVector
                );
            }

            return base.Draw(batcher, x, y);
        }
    }
}