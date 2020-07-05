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

        public exListBoxItem(DockerGame game)
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer,
                true);

            this.Guid = game.GUID;
            this.Image = game.Image;
            this.Name = game.Name;
            this.Text = game.Company;
            this.Enabled = game.Enabled;
        }

        public static void GrayScaleImage(Image mainImage)
        {
            Bitmap image = (Bitmap)mainImage;

            if (image == null)
                throw new ArgumentNullException("image");

            // lock the bitmap.
            var data = image.LockBits(
                          new Rectangle(0, 0, image.Width, image.Height),
                          ImageLockMode.ReadWrite, image.PixelFormat);
            try
            {
                unsafe
                {
                    // get a pointer to the data.
                    byte* ptr = (byte*)data.Scan0;

                    // loop over all the data.
                    for (int i = 0; i < data.Height; i++)
                    {
                        for (int j = 0; j < data.Width; j++)
                        {
                            // calculate the gray value.
                            byte y = (byte)(
                                (0.299 * ptr[2]) +
                                (0.587 * ptr[1]) +
                                (0.114 * ptr[0]));

                            // set the gray value.
                            ptr[0] = ptr[1] = ptr[2] = y;

                            // increment the pointer.
                            ptr += 3;
                        }

                        // move on to the next line.
                        ptr += data.Stride - data.Width * 3;
                    }
                }
            }
            finally
            {
                // unlock the bits when done or when 
                // an exception has been thrown.
                image.UnlockBits(data);
            }
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

            e.Graphics.FillRectangle(backBrush, e.Bounds);

            int ImageWidth = Math.Min(ListBox.ImageWidth, Image.Width);
            int ImageHeight = Math.Min(ListBox.ImageHeight, Image.Height);

            int TempX = (int)Math.Floor((double)ListBox.ImageWidth - (double)ImageWidth);
            int TempY = (int)Math.Floor((double)ListBox.ImageHeight - (double)ImageHeight);

            int ImageX = e.Bounds.X + TempX - (TempX / 2);
            int ImageY = e.Bounds.Y + TempY / 2;

            Image FinalImage = (Image)Image.Clone();

            if(!this.Enabled)
                GrayScaleImage(FinalImage);

            e.Graphics.DrawImage(FinalImage, ImageX, ImageY, ImageWidth, ImageHeight);

            Rectangle titleBounds = new Rectangle(e.Bounds.X + margin.Horizontal + ListBox.ItemHeight,
                                                  e.Bounds.Y + (ListBox.ItemHeight - titleFont.Height - detailsFont.Height) / 2,
                                                  e.Bounds.Width - margin.Right - margin.Horizontal,
                                                  ListBox.ItemHeight);
            Rectangle detailsBounds = new Rectangle(e.Bounds.X + 15 + margin.Horizontal + ListBox.ItemHeight,
                                                  e.Bounds.Y + (ListBox.ItemHeight - titleFont.Height + detailsFont.Height) / 2 + 1,
                                                  e.Bounds.Width - margin.Right - margin.Horizontal,
                                                  ListBox.ItemHeight);

            e.Graphics.DrawString(Name, titleFont, frontBrush, titleBounds, aligment);
            e.Graphics.DrawString(Text, detailsFont, frontBrush, detailsBounds, aligment);

            e.DrawFocusRectangle();
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
                ControlStyles.OptimizedDoubleBuffer,
                true);

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
            if (this.Items.Count > 0)
            {
                exListBoxItem item = (exListBoxItem)Items[e.Index];
                item.drawItem(e, Margin, format, this);
            }
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
