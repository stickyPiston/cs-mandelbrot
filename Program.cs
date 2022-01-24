using System;
using System.Drawing;
using System.Windows.Forms;

namespace Program {
    class Mandelbrot : Form {
        // Constants
        const int windowWidth = 1000;
        const int windowHeight = 600;

        const uint canvasWidth = 600;
        const uint canvasHeight = 600;
 
        readonly string[] colourSchemes = { "TI", "Black & White", "Christmas", "Aquamarine", "Random" };
        readonly string[] examples = { "Start", "Example 1", "Example 2", "Example 3" };

        // User-defined parameters
        uint limit = 400;
        double middleX = 0d;
        double middleY = 0d;
        double scale = .01;

        // UI
        TextBox middleXInput, middleYInput, scaleInput, limitInput;
        Button confirmButton;
        ListBox colourSelect, exampleSelect;

        // Random Variables
        readonly Random rand = new Random();
        readonly int randR, randG, randB;

        Mandelbrot() {
            this.Size = new Size(windowWidth, windowHeight);
            this.Text = "Mandelbrot";
            this.Paint += this.onPaint;
            this.MouseClick += this.onClick;

            initialiseUI();

            // Generate the values for the "random" colour
            randR = rand.Next(0, 127);
            randB = rand.Next(0, 255);
            randG = rand.Next(0, 255);
        }

        // UI-related funtions

        TextBox buildTextBox(int x, int y, int width, int height) {
            var t = new TextBox {
                Size = new Size(width, height),
                Location = new Point(x, y)
            };
            this.Controls.Add(t);
            return t;
        }

        Label buildLabel(int x, int y, int width, string text) {
            var l = new Label {
                Text = text,
                Location = new Point(x, y),
                Width = width,
                Font = new Font("Sans-Serif", 8)
            };
            this.Controls.Add(l);
            return l;
        }

        void initialiseUI() {
            const int inputAlignment = 100 + (int)canvasWidth;
            const int labelAlignment = 25 + (int)canvasWidth;

            var title = new Label {
                Text = "Mandelbrot Viewer 2000™",
                Location = new Point(labelAlignment, 27),
                Width = 200,
                Font = new Font("Sans-Serif", 12)
            };
            this.Controls.Add(title);

            // Inputs for middleX, middleY, scale & limit
            buildLabel(labelAlignment, 77, 75, "X:");
            middleXInput = buildTextBox(inputAlignment, 75, 200, 20);

            buildLabel(labelAlignment, 122, 75, "Y:");
            middleYInput = buildTextBox(inputAlignment, 120, 200, 20);

            buildLabel(labelAlignment, 167, 75, "Scale:");
            scaleInput = buildTextBox(inputAlignment, 165, 200, 20);

            buildLabel(labelAlignment, 212, 75, "Max. iterations:");
            limitInput = buildTextBox(inputAlignment, 210, 200, 20);

            // Input for colour
            buildLabel(labelAlignment, 305, 75, "Colour:");
            colourSelect = new ListBox {
                Location = new Point(inputAlignment, 305),
                Height = 100
            };

            foreach (string scheme in colourSchemes)
                colourSelect.Items.Add(scheme);
            colourSelect.SelectedIndex = 1;
            colourSelect.SelectedIndexChanged += (object _, EventArgs __) => { this.Invalidate(); };
            this.Controls.Add(colourSelect);

            // Input for examples
            buildLabel(labelAlignment, 410, 75, "Examples:");
            exampleSelect = new ListBox {
                Location = new Point(inputAlignment, 410),
                Height = 80
            };

            foreach (string scheme in examples)
                exampleSelect.Items.Add(scheme);
            exampleSelect.SelectedIndexChanged += this.onListChange;
            this.Controls.Add(exampleSelect);

            // "OK" Button
            confirmButton = new Button {
                Size = new Size(50, 25),
                Location = new Point(inputAlignment, 250),
                Text = "OK"
            };
            confirmButton.MouseClick += this.onButtonClick;
            this.Controls.Add(confirmButton);

            updateUI();
        }

        // Update the inputs in the UI with the stored values
        void updateUI() {
            middleXInput.Text = middleX.ToString();
            middleYInput.Text = middleY.ToString();
            scaleInput.Text = scale.ToString();
            limitInput.Text = limit.ToString();
        }

        // Return a colour for a pixel based on the mandelnumber and the selected colour palette
        Brush getColour(uint tries) {
            if (colourSelect.SelectedIndex == 0) {
                Brush result = Brushes.DarkGray;

                if (.8 * limit > tries)
                    result = Brushes.Gray;
                if (.6 * limit > tries)
                    result = Brushes.LightGray;
                if (.4 * limit > tries)
                    result = Brushes.LightBlue;
                if (.2 * limit > tries)
                    result = Brushes.Blue;
                if (.1 * limit > tries)
                    result = Brushes.Navy;
                if (.06 * limit > tries)
                    result = Brushes.Orange;
               if (.04 * limit > tries)
                    result = Brushes.Brown;
                if (.02 * limit > tries)
                    result = Brushes.Red;
                return result;
            } else if (colourSelect.SelectedIndex == 1) {
                if (tries == limit)
                    return Brushes.Black;
                if (tries % 2 == 0)
                    return Brushes.White;
                else
                    return Brushes.Black;
            } else if (colourSelect.SelectedIndex == 2) {
                return new SolidBrush(Color.FromArgb((int)tries % 32 * 8, (int)tries % 10 * 28, 0));
            } else if (colourSelect.SelectedIndex == 3) {
                return new SolidBrush(Color.FromArgb(0, (int)tries % 18 * 15, (int)tries % 32 * 8));
            } else if (colourSelect.SelectedIndex == 4) {
                return new SolidBrush(Color.FromArgb((int)tries % randR * (256 / randR), (int)tries % randG * (256 / randG), (int)tries % randB * (256 / randB)));
            }
            return Brushes.Black;
        }

        // Event handlers

        // Event handler for clicking the "OK" button
        void onButtonClick(object _, MouseEventArgs __) {
            try {
                middleX = double.Parse(middleXInput.Text);
                middleY = double.Parse(middleYInput.Text);
                scale = double.Parse(scaleInput.Text);
                limit = uint.Parse(limitInput.Text);

                this.Invalidate();
            } catch (Exception e) {
                MessageBox.Show("Could not read parameters: " + e.Message);
            }
        }

        // Event handler for click in the mandelbrot window
        void onClick(object _, MouseEventArgs mea) {
            if (mea.X < canvasWidth) {
                middleX = middleX - 0.5d * (scale * canvasWidth) + ((double)mea.X / canvasWidth * (scale * canvasWidth));
                middleY = middleY - 0.5d * (scale * canvasWidth) + ((double)(canvasHeight - mea.Y) / canvasWidth * (scale * canvasWidth));
                scale *= 0.5;

                updateUI();
                this.Invalidate();
            }
        }

        void onPaint(object _, PaintEventArgs pea) {
            Graphics gr = pea.Graphics;

            // Run mandelbrot function for every pixel in the canvas
            for (uint row = 0; row < canvasHeight; row++) {
                for (uint col = 0; col < canvasWidth; col++) {
                    double scaledSize = scale * canvasWidth;
                    double normX = scale * col + (middleX - scaledSize / 2);
                    double normY = scale * row + (middleY - scaledSize / 2);
                    double a = 0, b = 0;

                    uint tries = 0;
                    while (Math.Sqrt(a * a + b * b) < 2 && tries < limit) {
                        double oldA = a;
                        a = a * a - b * b + normX;
                        b = 2 * oldA * b + normY;
                        tries++;
                    }
 
                    gr.FillRectangle(getColour(tries), col, canvasHeight - row, 1, 1);
                }
            }
        }

        // Event handler for when the user clicks the colour/examples list
        void onListChange(object _, EventArgs ea) {
            if (exampleSelect.SelectedIndex == 0) {
                middleX = 0;
                middleY = 0;
                scale = 0.01;
            } else if (exampleSelect.SelectedIndex == 1) {
                middleX = -0.108625;
                middleY = 0.9014428;
                scale = 3.8147E-8;
            } else if (exampleSelect.SelectedIndex == 2) {
                middleX = -1.0079296875;
                middleY = 0.3112109375;
                scale = 1.953125E-5;
            } else if (exampleSelect.SelectedIndex == 3) {
                middleX = -0.1578125;
                middleY = 1.0328125;
                scale = 1.5625E-4;
            }

            updateUI();
            this.Invalidate();
        }
 
        static void Main() {
            Application.Run(new Mandelbrot());
        }
    }
}
