﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public class InformationPanel : GraphicObject
    {
        //private readonly List<(string text, Rectangle rectangle)> areas = new List<(string, Rectangle)>();
        private string text;
        private bool enabled;
        private readonly Action playSoundPanelShow;
        private readonly Action playSoundPanelHide;

        public InformationPanel(Image image, int width, int height, Point position, Action playSoundPanelShow, Action playSoundPanelHide, bool zoom = true)
            : base(image, width, height, position, 125, zoom)
        {
            this.playSoundPanelHide = playSoundPanelHide;
            this.playSoundPanelShow = playSoundPanelShow;
        }

        //public void Add(string text) => 
        //    areas.Add((text, new Rectangle(new Point(), new Size(Scale.Get(350), Scale.Get(50)))));

        public void Add(string text) => this.text += text + "\n\n";

        public void Add(string[] texts)
        {
            foreach (var text in texts)
                Add(text);
        }

        //public void Clear() => areas.Clear();
        public void Clear() => text = "";

        //public new void Paint(Graphics graphics)
        //{
        //    base.Paint(graphics);
        //    for (var i = 0; i < areas.Count; i++)
        //    {
        //        var rectangle = areas[i].rectangle;
        //        rectangle.Location = new Point(Position.X + Scale.Get(70), Position.Y + Scale.Get(40) + (Scale.Get(15) + rectangle.Height) * i);
        //        //graphics.DrawRectangle(StringStyle.Pen, rectangle);
        //        //graphics.FillRectangle(new SolidBrush(Color.FromArgb(240, 229, 181)), rectangle);
        //        graphics.DrawString(areas[i].text, StringStyle.TextFont, StringStyle.Black, rectangle);
        //    }
        //}

        public new void Paint(Graphics graphics)
        {
            base.Paint(graphics);
            var rectangle = new Rectangle(new Point(Position.X + Scale.Get(70), Position.Y + Scale.Get(40)), new Size(Scale.Get(350), 0));
            graphics.DrawString(text, StringStyle.TextFont, StringStyle.Black, rectangle);
        }

        public void Show()
        {
            if (enabled) return;
            GoRight();
        }

        public void Hide()
        {
            if (!enabled) return;
            enabled = false;
            GoLeft();
        }

        public void EveryTick()
        {
            Move();
            if (Position.X > 0)
            {
                Stop();
                Position = new Point(0, Position.Y);
                enabled = true;
                playSoundPanelShow();
            }
            else if (Position.X < -Bitmap.Width)
            {
                Stop();
                Position = new Point(-Bitmap.Width, Position.Y);
                playSoundPanelHide();
            }
        }
    }
}
