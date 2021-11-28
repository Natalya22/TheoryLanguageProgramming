namespace Translyator
{
  partial class Form1
  {
    /// <summary>
    /// Обязательная переменная конструктора.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Освободить все используемые ресурсы.
    /// </summary>
    /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Код, автоматически созданный конструктором форм Windows

    /// <summary>
    /// Требуемый метод для поддержки конструктора — не изменяйте 
    /// содержимое этого метода с помощью редактора кода.
    /// </summary>
    private void InitializeComponent()
    {
      this.UserCode = new System.Windows.Forms.RichTextBox();
      this.ProgrCode = new System.Windows.Forms.Label();
      this.WinOut = new System.Windows.Forms.Label();
      this.Output = new System.Windows.Forms.RichTextBox();
      this.Compile = new System.Windows.Forms.Button();
      this.Error = new System.Windows.Forms.RichTextBox();
      this.button1 = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // UserCode
      // 
      this.UserCode.Location = new System.Drawing.Point(12, 41);
      this.UserCode.Name = "UserCode";
      this.UserCode.Size = new System.Drawing.Size(378, 334);
      this.UserCode.TabIndex = 1;
      this.UserCode.Text = "";
      // 
      // ProgrCode
      // 
      this.ProgrCode.AutoSize = true;
      this.ProgrCode.Location = new System.Drawing.Point(12, 9);
      this.ProgrCode.Name = "ProgrCode";
      this.ProgrCode.Size = new System.Drawing.Size(110, 17);
      this.ProgrCode.TabIndex = 2;
      this.ProgrCode.Text = "Код программы";
      // 
      // WinOut
      // 
      this.WinOut.AutoSize = true;
      this.WinOut.Location = new System.Drawing.Point(425, 9);
      this.WinOut.Name = "WinOut";
      this.WinOut.Size = new System.Drawing.Size(94, 17);
      this.WinOut.TabIndex = 3;
      this.WinOut.Text = "Окно вывода";
      // 
      // Output
      // 
      this.Output.BackColor = System.Drawing.SystemColors.ControlLight;
      this.Output.Location = new System.Drawing.Point(405, 41);
      this.Output.Name = "Output";
      this.Output.ReadOnly = true;
      this.Output.Size = new System.Drawing.Size(359, 334);
      this.Output.TabIndex = 4;
      this.Output.Text = "";
      // 
      // Compile
      // 
      this.Compile.Location = new System.Drawing.Point(12, 390);
      this.Compile.Name = "Compile";
      this.Compile.Size = new System.Drawing.Size(140, 32);
      this.Compile.TabIndex = 5;
      this.Compile.Text = "Компилировать";
      this.Compile.UseVisualStyleBackColor = true;
      this.Compile.Click += new System.EventHandler(this.Compile_Click);
      // 
      // Error
      // 
      this.Error.Location = new System.Drawing.Point(295, 381);
      this.Error.Name = "Error";
      this.Error.Size = new System.Drawing.Size(469, 41);
      this.Error.TabIndex = 7;
      this.Error.Text = "";
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(158, 390);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(137, 32);
      this.button1.TabIndex = 8;
      this.button1.Text = "Очистить";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(829, 452);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.Error);
      this.Controls.Add(this.Compile);
      this.Controls.Add(this.Output);
      this.Controls.Add(this.WinOut);
      this.Controls.Add(this.ProgrCode);
      this.Controls.Add(this.UserCode);
      this.Name = "Form1";
      this.Text = "СУ-трансляция";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

        #endregion

        private System.Windows.Forms.RichTextBox UserCode;
        private System.Windows.Forms.Label ProgrCode;
        private System.Windows.Forms.Label WinOut;
        private System.Windows.Forms.RichTextBox Output;
        private System.Windows.Forms.Button Compile;
        private System.Windows.Forms.RichTextBox Error;
    private System.Windows.Forms.Button button1;
  }
}

