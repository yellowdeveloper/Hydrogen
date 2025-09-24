using Hydrogen.UserControls;

namespace Hydrogen
{
    partial class OverallForm
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OverallForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.main_cont = new System.Windows.Forms.Panel();
            this.side_cont = new System.Windows.Forms.Panel();
            this.header_cont = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.form_icon = new System.Windows.Forms.PictureBox();
            this.minimize_button = new System.Windows.Forms.Button();
            this.maximize_button = new System.Windows.Forms.Button();
            this.close_button = new System.Windows.Forms.Button();
            this.form_title = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.form_icon)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.header_cont, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 30);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1278, 718);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 300F));
            this.tableLayoutPanel2.Controls.Add(this.main_cont, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.side_cont, 1, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 43);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1272, 672);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // main_cont
            // 
            this.main_cont.Dock = System.Windows.Forms.DockStyle.Fill;
            this.main_cont.Location = new System.Drawing.Point(0, 0);
            this.main_cont.Margin = new System.Windows.Forms.Padding(0);
            this.main_cont.Name = "main_cont";
            this.main_cont.Padding = new System.Windows.Forms.Padding(5);
            this.main_cont.Size = new System.Drawing.Size(972, 672);
            this.main_cont.TabIndex = 0;
            // 
            // side_cont
            // 
            this.side_cont.Dock = System.Windows.Forms.DockStyle.Fill;
            this.side_cont.Location = new System.Drawing.Point(972, 0);
            this.side_cont.Margin = new System.Windows.Forms.Padding(0);
            this.side_cont.Name = "side_cont";
            this.side_cont.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.side_cont.Size = new System.Drawing.Size(300, 672);
            this.side_cont.TabIndex = 1;
            // 
            // header_cont
            // 
            this.header_cont.Dock = System.Windows.Forms.DockStyle.Fill;
            this.header_cont.Location = new System.Drawing.Point(0, 0);
            this.header_cont.Margin = new System.Windows.Forms.Padding(0);
            this.header_cont.Name = "header_cont";
            this.header_cont.Padding = new System.Windows.Forms.Padding(2, 5, 0, 0);
            this.header_cont.Size = new System.Drawing.Size(1278, 40);
            this.header_cont.TabIndex = 1;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tableLayoutPanel3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1280, 750);
            this.panel1.TabIndex = 1;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(1278, 748);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(77)))), ((int)(((byte)(128)))));
            this.tableLayoutPanel4.ColumnCount = 6;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 558F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel4.Controls.Add(this.form_icon, 0, 0);
            this.tableLayoutPanel4.Controls.Add(this.minimize_button, 3, 0);
            this.tableLayoutPanel4.Controls.Add(this.maximize_button, 4, 0);
            this.tableLayoutPanel4.Controls.Add(this.close_button, 5, 0);
            this.tableLayoutPanel4.Controls.Add(this.form_title, 1, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(1278, 30);
            this.tableLayoutPanel4.TabIndex = 1;
            this.tableLayoutPanel4.MouseDown += new System.Windows.Forms.MouseEventHandler(this.tableLayoutPanel4_MouseDown);
            this.tableLayoutPanel4.MouseMove += new System.Windows.Forms.MouseEventHandler(this.tableLayoutPanel4_MouseMove);
            // 
            // form_icon
            // 
            this.form_icon.Dock = System.Windows.Forms.DockStyle.Fill;
            this.form_icon.Image = global::Hydrogen.Properties.Resources.fydlogo1;
            this.form_icon.Location = new System.Drawing.Point(5, 5);
            this.form_icon.Margin = new System.Windows.Forms.Padding(5);
            this.form_icon.Name = "form_icon";
            this.form_icon.Size = new System.Drawing.Size(20, 20);
            this.form_icon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.form_icon.TabIndex = 0;
            this.form_icon.TabStop = false;
            // 
            // minimize_button
            // 
            this.minimize_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.minimize_button.FlatAppearance.BorderSize = 0;
            this.minimize_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.minimize_button.ForeColor = System.Drawing.Color.White;
            this.minimize_button.Location = new System.Drawing.Point(1191, 3);
            this.minimize_button.Name = "minimize_button";
            this.minimize_button.Size = new System.Drawing.Size(24, 24);
            this.minimize_button.TabIndex = 1;
            this.minimize_button.Text = "-";
            this.minimize_button.UseVisualStyleBackColor = true;
            this.minimize_button.Click += new System.EventHandler(this.minimize_button_Click);
            // 
            // maximize_button
            // 
            this.maximize_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.maximize_button.FlatAppearance.BorderSize = 0;
            this.maximize_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.maximize_button.ForeColor = System.Drawing.Color.White;
            this.maximize_button.Location = new System.Drawing.Point(1221, 3);
            this.maximize_button.Name = "maximize_button";
            this.maximize_button.Size = new System.Drawing.Size(24, 24);
            this.maximize_button.TabIndex = 2;
            this.maximize_button.Text = "ㅁ";
            this.maximize_button.UseVisualStyleBackColor = true;
            this.maximize_button.Click += new System.EventHandler(this.maximize_button_Click);
            // 
            // close_button
            // 
            this.close_button.Dock = System.Windows.Forms.DockStyle.Fill;
            this.close_button.FlatAppearance.BorderSize = 0;
            this.close_button.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Tomato;
            this.close_button.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Salmon;
            this.close_button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.close_button.ForeColor = System.Drawing.Color.White;
            this.close_button.Location = new System.Drawing.Point(1251, 3);
            this.close_button.Name = "close_button";
            this.close_button.Size = new System.Drawing.Size(24, 24);
            this.close_button.TabIndex = 3;
            this.close_button.Text = "x";
            this.close_button.UseVisualStyleBackColor = true;
            this.close_button.Click += new System.EventHandler(this.close_button_Click);
            // 
            // form_title
            // 
            this.form_title.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.form_title.AutoSize = true;
            this.form_title.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.form_title.ForeColor = System.Drawing.Color.White;
            this.form_title.Location = new System.Drawing.Point(31, 7);
            this.form_title.Margin = new System.Windows.Forms.Padding(1, 0, 0, 0);
            this.form_title.Name = "form_title";
            this.form_title.Size = new System.Drawing.Size(93, 15);
            this.form_title.TabIndex = 4;
            this.form_title.Text = "Hydrogen Sense";
            this.form_title.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OverallForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(242)))), ((int)(((byte)(249)))));
            this.ClientSize = new System.Drawing.Size(1280, 750);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1280, 720);
            this.Name = "OverallForm";
            this.Text = "Hydrogen Sense";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.form_icon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Panel main_cont;
        private System.Windows.Forms.Panel side_cont;
        private System.Windows.Forms.Panel header_cont;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.PictureBox form_icon;
        private System.Windows.Forms.Button minimize_button;
        private System.Windows.Forms.Button maximize_button;
        private System.Windows.Forms.Button close_button;
        private System.Windows.Forms.Label form_title;
    }
}

