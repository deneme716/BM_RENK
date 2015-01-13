using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI;

using AForge.Math;
using AForge.Imaging;

namespace IPLab
{
    /// <summary>
    /// Summary description for ImageStatisticsWindow.
    /// </summary>
    /// 
    public class ImageStatisticsWindow : Content
    {
        private System.Windows.Forms.PropertyGrid propertyGrid;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        private int currentImageHash = 0;
        public int kirlilik;

        // Constructor
        public ImageStatisticsWindow( )
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent( );

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if ( disposing )
            {
                if ( components != null )
                {
                    components.Dispose( );
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent( )
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageStatisticsWindow));
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.SuspendLayout();
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.HelpVisible = false;
            this.propertyGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(270, 256);
            this.propertyGrid.TabIndex = 0;
            this.propertyGrid.ToolbarVisible = false;
            // 
            // ImageStatisticsWindow
            // 
            this.AllowedStates = ((WeifenLuo.WinFormsUI.ContentStates)(((WeifenLuo.WinFormsUI.ContentStates.Float | WeifenLuo.WinFormsUI.ContentStates.DockLeft)
                        | WeifenLuo.WinFormsUI.ContentStates.DockRight)));
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(270, 255);
            this.CloseButton = false;
            this.Controls.Add(this.propertyGrid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(276, 280);
            this.Name = "ImageStatisticsWindow";
            this.ShowInTaskbar = false;
            this.Text = "Kir Ýstatistiði";
            this.ResumeLayout(false);

        }
        #endregion

        // Gather image statistics
        public void GatherStatistics( Bitmap image )
        {
            // avoid calculation in the case of the same image
            if ( image != null )
            {
                if ( currentImageHash == image.GetHashCode( ) )
                    return;
                currentImageHash = image.GetHashCode( );
            }
            else
            {
                propertyGrid.SelectedObject = null;
                return;
            }

            System.Diagnostics.Debug.WriteLine( "--- Gathering stastics" );

            // check pixel format
            if ( image.PixelFormat == PixelFormat.Format8bppIndexed )
            {
                // busy
                Capture = true;
                Cursor = Cursors.WaitCursor;

                ColorImageStatisticsDescription statDesc = new ColorImageStatisticsDescription( image );
                // show statistics
                propertyGrid.SelectedObject = statDesc;
                propertyGrid.ExpandAllGridItems( );

                // free
                Cursor = Cursors.Arrow;
                Capture = false;
            }
            else
            {
                propertyGrid.SelectedObject = null;
            }
        }


    }

    // Helper class to display image statistics
    internal class ColorImageStatisticsDescription
    {
        private ImageStatistics statRGB;
       
        private ImageStatisticsHSL statHSL;
        private ImageStatisticsYCbCr statYCbCr;

        // General with black ------------------------------------
        // Total pixels count
        [Category( "Sonuç" )]
        public int Piksel_Toplam
        {
            get { return statRGB.PixelsCount; }
        }
        // Pixels without black
        [Category("Sonuç")]
        public int Piksel_Beyaz
        {
            get { return statRGB.PixelsCountWithoutBlack; }
        }
        [Category("Sonuç")]
        public double Kirlilik
        {
            get { return 100 - (Convert.ToDouble(statRGB.PixelsCountWithoutBlack) * 100 / Convert.ToDouble(statRGB.PixelsCount)); }
        }



        // Constructor
        public ColorImageStatisticsDescription( Bitmap image )
        {
            // get image dimension
            int width = image.Width;
            int height = image.Height;

            // lock it
            BitmapData imgData = image.LockBits( new Rectangle( 0, 0, width, height ), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb );

            // gather statistics
            statRGB = new ImageStatistics( imgData );
            statHSL = new ImageStatisticsHSL( imgData );
            statYCbCr = new ImageStatisticsYCbCr( imgData );

            // unlock image
            image.UnlockBits( imgData );
        }
    }
}
