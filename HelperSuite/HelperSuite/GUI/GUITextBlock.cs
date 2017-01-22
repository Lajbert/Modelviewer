﻿using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HelperSuite.HelperSuite.GUI
{
    /// <summary>
    /// Just a colored block with a text inside
    /// </summary>
    public class GUITextBlock : GUIBlock
    {
        public Color TextColor;
        public SpriteFont TextFont;

        public override Vector2 Dimensions
        {
            get { return _dimensions; }
            set
            {
                _dimensions = value;
                ComputeFontPosition();
            }
        }

        public StringBuilder Text
        {
            get { return _text; }
            set
            {
                _text = value; 
                ComputeFontPosition();
            }
        }
        private StringBuilder _text;

        private Vector2 _fontPosition;
        protected GUIStyle.TextAlignment _textAlignment;
        protected Vector2 _textBorder = new Vector2(10,1);
        private Vector2 _dimensions;

        public GUITextBlock(Vector2 position, Vector2 dimensions, String text, SpriteFont font, Color blockColor, Color textColor, GUIStyle.TextAlignment textAlignment = GUIStyle.TextAlignment.Left, Vector2 textBorder = default(Vector2), int layer = 0, GUIStyle.GUIAlignment alignment = GUIStyle.GUIAlignment.None, Vector2 parentDimensions = default(Vector2)) : base(position,dimensions, blockColor, layer, alignment, parentDimensions)
        {
            _text = new StringBuilder(text);
            TextColor = textColor;
            TextFont = font;
            _textAlignment = textAlignment;

            if (textBorder != default(Vector2))
            {
                _textBorder = textBorder;
            }

            ComputeFontPosition();
        }

        protected virtual void FontWrap(ref Vector2 textDimension, Vector2 blockDimensions)
        {
            float textwidth = textDimension.X;
            float characterwidth = textwidth / _text.Length;
            int charactersperline = (int)((blockDimensions.X - _textBorder.X * 2) / characterwidth);

            int charactersprocessed = _text.Length;
            int lines = 1;
            while (charactersperline < charactersprocessed)
            {
                _text.Insert(charactersperline * lines + lines, '\n');
                charactersprocessed -= charactersperline;
                lines++;
            }

            if (textDimension.Y * lines + 2 * _textBorder.Y > Dimensions.Y)
            {
                _dimensions = new Vector2(_dimensions.X, textDimension.Y * lines + 2 * _textBorder.Y);
                textDimension = new Vector2(charactersperline * characterwidth, textDimension.Y * lines);
            }
        }

        protected virtual void ComputeFontPosition()
        {
            if (_text == null) return;
            Vector2 textDimension = TextFont.MeasureString(_text);

            //Let's check wrap!

            FontWrap(ref textDimension, Dimensions);

            switch (_textAlignment)
            {
                case GUIStyle.TextAlignment.Left:
                    _fontPosition = Dimensions * 0.5f * Vector2.UnitY + _textBorder * Vector2.UnitX - textDimension * 0.5f * Vector2.UnitY;
                    break;
                case GUIStyle.TextAlignment.Center:
                    _fontPosition = Dimensions * 0.5f - textDimension * 0.5f;
                    break;
                case GUIStyle.TextAlignment.Right:
                    _fontPosition = Dimensions * new Vector2(1, 0.5f) - _textBorder * Vector2.UnitX - textDimension * 0.5f;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public override void Draw(GUIRenderer.GUIRenderer guiRenderer, Vector2 parentPosition)
        {
            guiRenderer.DrawQuad(parentPosition + Position, Dimensions, Color);
            guiRenderer.DrawText(parentPosition + Position + _fontPosition, _text, TextFont, TextColor);
        }
        
    }
}