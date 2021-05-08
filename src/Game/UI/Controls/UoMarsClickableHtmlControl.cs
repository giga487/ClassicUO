using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClassicUO.Game.Scenes;
using ClassicUO.Input;
using ClassicUO.IO.Resources;
using ClassicUO.Renderer;
using ClassicUO.Utility;
using ClassicUO.Utility.Logging;
using ClassicUO.Utility.Platforms;
using Microsoft.Xna.Framework;

namespace ClassicUO.Game.UI.Controls
{
    internal class UoMarsClickableHtmlControl : Control
    {
        private RenderedText _gameText;
        
        private RenderedText _gameTextNormal;
        private RenderedText _gameTextOver;
        private ScrollBarBase _scrollBar;
        private int _buttonId;

        public UoMarsClickableHtmlControl(List<string> parts, string[] lines)
        {
            X = int.Parse(parts[1]);
            Y = int.Parse(parts[2]);
            Width = int.Parse(parts[3]);
            Height = int.Parse(parts[4]);
            int textIndex = int.Parse(parts[5]);
            int textOverIndex = int.Parse(parts[6]);
            _buttonId = int.Parse(parts[7]);

            _gameTextNormal = RenderedText.Create(string.Empty, isunicode: true, font: 1);
            _gameTextNormal.IsHTML = true;
            _gameTextNormal.MaxWidth = Width;

            _gameText = _gameTextNormal;
            
            _gameTextOver = RenderedText.Create(string.Empty, isunicode: true, font: 1);
            _gameTextOver.IsHTML = true;
            _gameTextOver.MaxWidth = Width;
            
            CanMove = true;
            UseFlagScrollbar = false;
            IsFromServer = true;

            if (textIndex >= 0 && textIndex < lines.Length)
            {
                InternalBuild(lines[textIndex], _gameTextNormal, 0);
            }
            
            if (textOverIndex >= 0 && textOverIndex < lines.Length)
            {
                InternalBuild(lines[textOverIndex], _gameTextOver, 0);
            }
        }
        
        public bool UseFlagScrollbar { get; }

        public int ScrollX { get; set; }

        public int ScrollY { get; set; }

        private void InternalBuild(string text, RenderedText internalText, int hue)
        {
            if (!string.IsNullOrEmpty(text))
            {
                uint htmlColor =0x010101FF;
                ushort color = 0xFFFF;
                internalText.MaxWidth -= 9;
                internalText.HTMLColor = htmlColor;
                internalText.Hue = color;

                internalText.HasBackgroundColor = true;
                internalText.Text = text;
            }
        }

        public override bool Draw(UltimaBatcher2D batcher, int x, int y)
        {
            if (IsDisposed)
            {
                return false;
            }

            ResetHueVector();

            Rectangle scissor = ScissorStack.CalculateScissors
            (
                Matrix.Identity,
                x,
                y,
                Width,
                Height
            );

            if (ScissorStack.PushScissors(batcher.GraphicsDevice, scissor))
            {
                batcher.EnableScissorTest(true);
                base.Draw(batcher, x, y);

                _gameText.Draw
                (
                    batcher,
                    Width + ScrollX,
                    Height + ScrollY,
                    x,
                    y,
                    Width,
                    Height,
                    ScrollX,
                    ScrollY
                );

                batcher.EnableScissorTest(false);
                ScissorStack.PopScissors(batcher.GraphicsDevice);
            }

            return true;
        }
        
        protected override void OnMouseEnter(int x, int y)
        {
            _gameText = _gameTextOver;
        }

        protected override void OnMouseExit(int x, int y)
        {
            _gameText = _gameTextNormal;
        }

        protected override void OnMouseUp(int x, int y, MouseButtonType button)
        {
            if (button == MouseButtonType.Left)
            {
                if (Client.Game.Scene is GameScene)
                {
                    OnButtonClick(_buttonId);

                    Mouse.LastLeftButtonClickTime = 0;
                    Mouse.CancelDoubleClick = true;
                }
            }
        }

        public override void Dispose()
        {
            _gameText?.Destroy();
            _gameTextNormal?.Destroy();
            _gameTextOver?.Destroy();
            
            _gameText = null;
            _gameTextNormal = null;
            _gameTextOver = null;
            
            base.Dispose();
        }
    }
}