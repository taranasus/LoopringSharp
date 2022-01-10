namespace LoopringSharp.OneButtonPayExample
{
    partial class TransferForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnMakePayment = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cbEnvironment = new System.Windows.Forms.ComboBox();
            this.cbPaymentToken = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.connectionStatus = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.nudPaymentAmmount = new System.Windows.Forms.NumericUpDown();
            this.cbPaymentFeeToken = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbPayeeAddress = new System.Windows.Forms.TextBox();
            this.lbTransactionFee = new System.Windows.Forms.Label();
            this.tbMemo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudPaymentAmmount)).BeginInit();
            this.SuspendLayout();
            // 
            // btnMakePayment
            // 
            this.btnMakePayment.Enabled = false;
            this.btnMakePayment.Location = new System.Drawing.Point(12, 356);
            this.btnMakePayment.Name = "btnMakePayment";
            this.btnMakePayment.Size = new System.Drawing.Size(284, 34);
            this.btnMakePayment.TabIndex = 0;
            this.btnMakePayment.Text = "Make Payment";
            this.btnMakePayment.UseVisualStyleBackColor = true;
            this.btnMakePayment.Click += new System.EventHandler(this.btnMakePayment_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(284, 49);
            this.label2.TabIndex = 2;
            this.label2.Text = "This is a quick demo on how to do a transfer of coin using the API. The code in t" +
    "he form is explained in detail so enjoy using it";
            // 
            // cbEnvironment
            // 
            this.cbEnvironment.FormattingEnabled = true;
            this.cbEnvironment.Items.AddRange(new object[] {
            "Real | https://api.loopring.network/",
            "TestNet | https://uat2.loopring.io/"});
            this.cbEnvironment.Location = new System.Drawing.Point(12, 61);
            this.cbEnvironment.Name = "cbEnvironment";
            this.cbEnvironment.Size = new System.Drawing.Size(284, 23);
            this.cbEnvironment.TabIndex = 3;
            this.cbEnvironment.SelectedIndexChanged += new System.EventHandler(this.cbEnvironment_SelectedIndexChanged);
            // 
            // cbPaymentToken
            // 
            this.cbPaymentToken.Enabled = false;
            this.cbPaymentToken.FormattingEnabled = true;
            this.cbPaymentToken.Location = new System.Drawing.Point(12, 194);
            this.cbPaymentToken.Name = "cbPaymentToken";
            this.cbPaymentToken.Size = new System.Drawing.Size(90, 23);
            this.cbPaymentToken.TabIndex = 4;
            this.cbPaymentToken.SelectedIndexChanged += new System.EventHandler(this.cbPaymentToken_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 176);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "Payment Token";
            // 
            // connectionStatus
            // 
            this.connectionStatus.AutoSize = true;
            this.connectionStatus.Location = new System.Drawing.Point(12, 87);
            this.connectionStatus.Name = "connectionStatus";
            this.connectionStatus.Size = new System.Drawing.Size(117, 15);
            this.connectionStatus.TabIndex = 7;
            this.connectionStatus.Text = "Status: Disconnected";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(120, 176);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "Amount";
            // 
            // nudPaymentAmmount
            // 
            this.nudPaymentAmmount.DecimalPlaces = 8;
            this.nudPaymentAmmount.Enabled = false;
            this.nudPaymentAmmount.Location = new System.Drawing.Point(120, 194);
            this.nudPaymentAmmount.Name = "nudPaymentAmmount";
            this.nudPaymentAmmount.Size = new System.Drawing.Size(176, 23);
            this.nudPaymentAmmount.TabIndex = 9;
            this.nudPaymentAmmount.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // cbPaymentFeeToken
            // 
            this.cbPaymentFeeToken.Enabled = false;
            this.cbPaymentFeeToken.FormattingEnabled = true;
            this.cbPaymentFeeToken.Location = new System.Drawing.Point(12, 250);
            this.cbPaymentFeeToken.Name = "cbPaymentFeeToken";
            this.cbPaymentFeeToken.Size = new System.Drawing.Size(90, 23);
            this.cbPaymentFeeToken.TabIndex = 10;
            this.cbPaymentFeeToken.SelectedIndexChanged += new System.EventHandler(this.cbPaymentFeeToken_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 232);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 15);
            this.label4.TabIndex = 11;
            this.label4.Text = "Payment Fee Token";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 116);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 15);
            this.label5.TabIndex = 12;
            this.label5.Text = "Receiver Address:";
            // 
            // tbPayeeAddress
            // 
            this.tbPayeeAddress.Enabled = false;
            this.tbPayeeAddress.Location = new System.Drawing.Point(12, 134);
            this.tbPayeeAddress.Name = "tbPayeeAddress";
            this.tbPayeeAddress.Size = new System.Drawing.Size(284, 23);
            this.tbPayeeAddress.TabIndex = 13;
            this.tbPayeeAddress.Text = "0x865281bF6cF78060d18E71aedaA9a5c9532B947a";
            // 
            // lbTransactionFee
            // 
            this.lbTransactionFee.AutoSize = true;
            this.lbTransactionFee.Location = new System.Drawing.Point(120, 253);
            this.lbTransactionFee.Name = "lbTransactionFee";
            this.lbTransactionFee.Size = new System.Drawing.Size(91, 15);
            this.lbTransactionFee.TabIndex = 14;
            this.lbTransactionFee.Text = "Transaction Fee:";
            // 
            // tbMemo
            // 
            this.tbMemo.Location = new System.Drawing.Point(12, 327);
            this.tbMemo.Name = "tbMemo";
            this.tbMemo.Size = new System.Drawing.Size(284, 23);
            this.tbMemo.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 309);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(45, 15);
            this.label6.TabIndex = 17;
            this.label6.Text = "Memo:";
            // 
            // TransferForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(308, 406);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.tbMemo);
            this.Controls.Add(this.lbTransactionFee);
            this.Controls.Add(this.tbPayeeAddress);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cbPaymentFeeToken);
            this.Controls.Add(this.nudPaymentAmmount);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.connectionStatus);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbPaymentToken);
            this.Controls.Add(this.cbEnvironment);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnMakePayment);
            this.Name = "TransferForm";
            this.Text = "Transfer LRC";
            ((System.ComponentModel.ISupportInitialize)(this.nudPaymentAmmount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button btnMakePayment;
        private Label label2;
        private ComboBox cbEnvironment;
        private ComboBox cbPaymentToken;
        private Label label1;
        private Label connectionStatus;
        private Label label3;
        private NumericUpDown nudPaymentAmmount;
        private ComboBox cbPaymentFeeToken;
        private Label label4;
        private Label label5;
        private TextBox tbPayeeAddress;
        private Label lbTransactionFee;
        private Button btnConnect;
        private TextBox tbMemo;
        private Label label6;
    }
}