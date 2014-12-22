using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace ContextBoundSample
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class Mainform : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox ResultText;
		private System.Windows.Forms.Button NoCatchButton;
		private System.Windows.Forms.Button SwallowExceptionButton;
		private System.Windows.Forms.Button WriteExceptionButton;
		private System.Windows.Forms.Button BothButton;
		private ContextBoundSample.SampleObj _SampleObj;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Mainform()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			_SampleObj = new SampleObj();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.NoCatchButton = new System.Windows.Forms.Button();
			this.SwallowExceptionButton = new System.Windows.Forms.Button();
			this.WriteExceptionButton = new System.Windows.Forms.Button();
			this.BothButton = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.ResultText = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// NoCatchButton
			// 
			this.NoCatchButton.Location = new System.Drawing.Point(10, 64);
			this.NoCatchButton.Name = "NoCatchButton";
			this.NoCatchButton.Size = new System.Drawing.Size(88, 32);
			this.NoCatchButton.TabIndex = 0;
			this.NoCatchButton.Text = "No Catch";
			this.NoCatchButton.Click += new System.EventHandler(this.NoCatchButton_Click);
			// 
			// SwallowExceptionButton
			// 
			this.SwallowExceptionButton.Location = new System.Drawing.Point(103, 64);
			this.SwallowExceptionButton.Name = "SwallowExceptionButton";
			this.SwallowExceptionButton.Size = new System.Drawing.Size(88, 32);
			this.SwallowExceptionButton.TabIndex = 1;
			this.SwallowExceptionButton.Text = "Swallow Exception";
			this.SwallowExceptionButton.Click += new System.EventHandler(this.SwallowExceptionButton_Click);
			// 
			// WriteExceptionButton
			// 
			this.WriteExceptionButton.Location = new System.Drawing.Point(196, 64);
			this.WriteExceptionButton.Name = "WriteExceptionButton";
			this.WriteExceptionButton.Size = new System.Drawing.Size(88, 32);
			this.WriteExceptionButton.TabIndex = 2;
			this.WriteExceptionButton.Text = "Write Exception";
			this.WriteExceptionButton.Click += new System.EventHandler(this.WriteExceptionButton_Click);
			// 
			// BothButton
			// 
			this.BothButton.Location = new System.Drawing.Point(289, 64);
			this.BothButton.Name = "BothButton";
			this.BothButton.Size = new System.Drawing.Size(88, 32);
			this.BothButton.TabIndex = 3;
			this.BothButton.Text = "Both";
			this.BothButton.Click += new System.EventHandler(this.BothButton_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(5, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(376, 48);
			this.label3.TabIndex = 8;
			this.label3.Text = "Use the buttons below to test the automatic exception handling on the sample cont" +
				"ext-bound object.  Each button does something a little different.  Feel free to " +
				"play around with the sample to change the configuration.";
			// 
			// ResultText
			// 
			this.ResultText.Location = new System.Drawing.Point(8, 104);
			this.ResultText.Multiline = true;
			this.ResultText.Name = "ResultText";
			this.ResultText.Size = new System.Drawing.Size(368, 24);
			this.ResultText.TabIndex = 9;
			this.ResultText.Text = "";
			// 
			// Mainform
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(386, 135);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.ResultText,
																		  this.label3,
																		  this.BothButton,
																		  this.WriteExceptionButton,
																		  this.SwallowExceptionButton,
																		  this.NoCatchButton});
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Mainform";
			this.Text = "Context-Bound Object Sample";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new Mainform());
		}

		private void NoCatchButton_Click(Object sender, System.EventArgs e)
		{
			String resultMessage = String.Empty;

			try
			{
				String returnValue = _SampleObj.NoCatch();
				resultMessage = String.Format("Returned value: {0}", returnValue);				
			}
			catch (ArgumentException ae)
			{
				resultMessage = String.Format("Caught server side error: {0}", ae.Message.ToString());
			}
			catch (Exception ex)
			{
				resultMessage = String.Format("Caught client side error: {0}", ex.Message.ToString());
			}
			finally
			{
				ResultText.Text = resultMessage;
			}
		}

		private void SwallowExceptionButton_Click(Object sender, System.EventArgs e)
		{
			String resultMessage = String.Empty;

			try
			{
				String returnValue = _SampleObj.SwallowException();
				resultMessage = String.Format("Returned value: {0}", returnValue);				
			}
			catch (ArgumentException ae)
			{
				resultMessage = String.Format("Caught server side error: {0}", ae.Message.ToString());
			}
			catch (Exception ex)
			{
				resultMessage = String.Format("Caught client side error: {0}", ex.Message.ToString());
			}
			finally
			{
				ResultText.Text = resultMessage;
			}
		}

		private void WriteExceptionButton_Click(Object sender, System.EventArgs e)
		{
			String resultMessage = String.Empty;

			try
			{
				String returnValue = _SampleObj.WriteException();
				resultMessage = String.Format("Returned value: {0}", returnValue);				
			}
			catch (ArgumentException ae)
			{
				resultMessage = String.Format("Caught server side error: {0}", ae.Message.ToString());
			}
			catch (Exception ex)
			{
				resultMessage = String.Format("Caught client side error: {0}", ex.Message.ToString());
			}
			finally
			{
				ResultText.Text = resultMessage;
			}
		}

		private void BothButton_Click(Object sender, System.EventArgs e)
		{
			String resultMessage = String.Empty;

			try
			{
				String returnValue = _SampleObj.Both();
				resultMessage = String.Format("Returned value: {0}", returnValue);				
			}
			catch (ArgumentException ae)
			{
				resultMessage = String.Format("Caught server side error: {0}", ae.Message.ToString());
			}
			catch (Exception ex)
			{
				resultMessage = String.Format("Caught client side error: {0}", ex.Message.ToString());
			}
			finally
			{
				ResultText.Text = resultMessage;
			}
		}
	}
}
