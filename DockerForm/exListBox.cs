using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace DockerForm
{
    public partial class exListBoxItem : Control
    {
        public string Guid;
        public Image Image;
        private PlatformCode platform;

        public exListBoxItem(DockerGame game)
        {
            this.Guid = game.GUID;
            this.Image = game.Image;
            this.Name = game.Name;
            this.Text = game.Company;
            this.Enabled = game.Enabled;
            this.platform = game.Platform;
        }

        public void drawItem(DrawItemEventArgs e, Padding margin, StringFormat aligment, exListBox ListBox)
        {
            
            SolidBrush backBrush = new SolidBrush(Color.FromArgb(255, 255, 255));
            SolidBrush frontBrush = new SolidBrush(Color.FromArgb(0, 0, 0));

            Font titleFont = new Font("Segoe UI", 10, FontStyle.Regular);
            Font detailsFont = new Font("Segoe UI", 8, FontStyle.Regular);

            if (e.Index % 2 == 0)
                backBrush = new SolidBrush(Color.FromArgb(255, 255, 255));
            else
                backBrush = new SolidBrush(Color.FromArgb(241, 241, 241));

            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                backBrush = new SolidBrush(Color.FromArgb(196, 224, 247));
                frontBrush = new SolidBrush(Color.FromArgb(0, 0, 0));
            }

            int ImageWidth = Math.Min(ListBox.ImageWidth, Image.Width);
            int ImageHeight = Math.Min(ListBox.ImageHeight, Image.Height);

            int TempX = (int)Math.Floor((double)ListBox.ImageWidth - (double)ImageWidth);
            int TempY = (int)Math.Floor((double)ListBox.ImageHeight - (double)ImageHeight);

            int ImageX = 0 + TempX - (TempX / 2);
            int ImageY = 0 + TempY / 2;

            Rectangle titleBounds = new Rectangle(e.Bounds.X + margin.Horizontal + ListBox.ItemHeight,
                                                  0 + (ListBox.ItemHeight - titleFont.Height - detailsFont.Height) / 2,
                                                  e.Bounds.Width - margin.Right - margin.Horizontal,
                                                  ListBox.ItemHeight);
            Rectangle detailsBounds = new Rectangle(e.Bounds.X + 15 + margin.Horizontal + ListBox.ItemHeight,
                                                  0 + (ListBox.ItemHeight - titleFont.Height + detailsFont.Height) / 2 + 1,
                                                  e.Bounds.Width - margin.Right - margin.Horizontal,
                                                  ListBox.ItemHeight);

            var bmp = new Bitmap(e.Bounds.Width, e.Bounds.Height);
            using (var g = Graphics.FromImage(bmp))
            {
                g.FillRectangle(backBrush, 0, 0, e.Bounds.Width, e.Bounds.Height);
                g.DrawImage(Image, ImageX, ImageY, ImageWidth, ImageHeight);
                g.DrawString(Name, titleFont, frontBrush, titleBounds, aligment);
                g.DrawString(Text, detailsFont, frontBrush, detailsBounds, aligment);

                if (!Enabled)
                    g.FillRectangle(new SolidBrush(Color.FromArgb(150, 250, 250, 250)), 0, 0, ListBox.ImageWidth, ListBox.ImageHeight);

                if (platform == PlatformCode.Microsoft)
                    g.DrawImage(Properties.Resources.logo_microsoft, 4, 4, ImageWidth / 4, ImageWidth / 4);
            }

            e.Graphics.DrawImage(bmp, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);
        }
    }

    public partial class exListBox : ListBox
    {
        private StringFormat format;
        public int ImageHeight;
        public int ImageWidth;

        public exListBox()
        {
            this.SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.DoubleBuffer, true);

            this.format = new StringFormat();
            this.format.Alignment = StringAlignment.Near;
            this.format.LineAlignment = StringAlignment.Near;
        }

        public void SetSize(int ImageHeight, int ImageWidth)
        {
            this.ItemHeight = ImageHeight;
            this.ImageHeight = ImageHeight;
            this.ImageWidth = ImageWidth;
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            try
            {
                exListBoxItem item = (exListBoxItem)Items[e.Index];
                item.drawItem(e, Margin, format, this);
            }
            catch (Exception) { }
        }

        public exListBoxItem GetItemFromGuid(string guid)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                exListBoxItem item = (exListBoxItem)Items[i];
                if (item.Guid == guid)
                    return item;
            }
            return null;
        }

        public int GetIndexFromGuid(string guid)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                exListBoxItem item = (exListBoxItem)Items[i];
                if (item.Guid == guid)
                    return i;
            }
            return -1;
        }

        public void Sort()
        {
            if (Items.Count > 1)
            {
                bool swapped;
                do
                {
                    int counter = Items.Count - 1;
                    swapped = false;

                    while (counter > 0)
                    {
                        exListBoxItem ItemNext = (exListBoxItem)Items[counter];
                        exListBoxItem ItemPrev = (exListBoxItem)Items[counter - 1];

                        if (String.Compare(ItemNext.Name, ItemPrev.Name) < 0)
                        {
                            object temp = Items[counter];
                            Items[counter] = Items[counter - 1];
                            Items[counter - 1] = temp;
                            swapped = true;
                        }

                        counter -= 1;
                    }
                }
                while ((swapped == true));
            }
        }
    }
}
