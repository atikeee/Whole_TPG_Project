using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
namespace TestPlanGen
{
    public class ComboBoxToolTip : ComboBox
    {
        private ToolTip itemToolTip;

        public ComboBoxToolTip()
        {
            this.DrawMode = DrawMode.OwnerDrawFixed;
            this.DrawItem += new DrawItemEventHandler(DrawItemInList);
            this.DropDownClosed += new EventHandler(DropDownListIsClosed);
            this.itemToolTip = new ToolTip();
        }

        void DropDownListIsClosed(object sender, EventArgs e)
        {
            this.itemToolTip.Hide(this);
        }

        private bool IsTruncated(string text, Graphics graphics, Font font, Rectangle destinationBounds)
        {
            SizeF size = graphics.MeasureString(text, font);
            Rectangle offsetToZeroRect = new Rectangle(0, 0, destinationBounds.Width, destinationBounds.Height);

            return !offsetToZeroRect.Contains((int)size.Width, (int)size.Height);
        }

        void DrawItemInList(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();

            string text = this.Items[e.Index].ToString();
            bool truncated;

            using (Graphics graphics = e.Graphics)
            {
                graphics.DrawString(text, e.Font, Brushes.Black, e.Bounds, StringFormat.GenericDefault);
                truncated = IsTruncated(text, graphics, e.Font, e.Bounds);
            }

            if ((e.State == DrawItemState.Selected) && (truncated))
            {
                this.itemToolTip.Show(text, this, e.Bounds.X + e.Bounds.Width, e.Bounds.Y + e.Bounds.Height);
            }
            else
            {
                this.itemToolTip.Hide(this);
            }

            e.DrawFocusRectangle();
        }
    }
}
