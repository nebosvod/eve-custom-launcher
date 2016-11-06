using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace eveCustomLauncher
{
    public partial class frmMain : Form
    {
        static bool lockForm = false;
        static bool characterChallenge = false;
        private Action<Exception> action = (Exception e) => new ErrorForm(e).ShowDialog();
        private EveLauncher launcher;
        public frmMain(EveLauncher launcher)
        {
            this.launcher = launcher;
            InitializeComponent();
            characterChallengeTab.Parent = null;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            UpdateSettingsProfilesList();
        }

        private void UpdateSettingsProfilesList()
        {
            cmbSettings.Items.Clear();
            cmbSettings.Items.AddRange(EveLauncher.GetSettingsProfilesList());
            cmbSettings.Items.Add("<add new profile>");
            cmbSettings.SelectedIndex = 0;
        }

        private void cmbSettings_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbSettings.SelectedIndex == cmbSettings.Items.Count - 1)
            {
                cmbSettings.Visible = false;
                txtNewProfile.Visible = true;
                txtNewProfile.Select();
            }
        }

        private void txtNewProfile_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                EveLauncher.CreateSettingsProfile(txtNewProfile.Text);
                txtNewProfile.Visible = false;
                cmbSettings.Visible = true;
                UpdateSettingsProfilesList();
                cmbSettings.SelectedItem = txtNewProfile.Text;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                txtNewProfile.Text = string.Empty;
                txtNewProfile.Visible = false;
                cmbSettings.Visible = true;
                cmbSettings.SelectedIndex = 0;
            }
            else if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) || (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9) || e.KeyCode == Keys.Back || e.KeyCode == Keys.Delete || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Home || e.KeyCode == Keys.End)
            {
                e.SuppressKeyPress = false;
            }
            else e.SuppressKeyPress = true;
        }

        private bool CheckCharacterNameField()
        {
            ToolTip tip = new ToolTip();
            tip.IsBalloon = true;

            if (txtCharacterName.Text == string.Empty)
            {
                tip.Show("this field cannot be empty", txtCharacterName, new Point(-10, -35), 5000);
                return false;
            }
            else return true;
        }

        private bool CheckFields()
        {
            ToolTip tip = new ToolTip();
            tip.IsBalloon = true;

            if (txtUsername.Text == string.Empty)
            {
                tip.Show("this field cannot be empty", txtUsername, new Point(-10, -35), 5000);
                return false;
            }

            else if (txtPassword.Text == string.Empty)
            {
                tip.Show("this field cannot be empty", txtPassword, new Point(-10, -35), 5000);
                return false;
            }

            else if (txtNewProfile.Visible == true)
            {
                tip.Show("enter new profile name and press \"Enter\" to save it", txtNewProfile, new Point(-10, -35), 5000);
                return false;
            }

            else if (cmbSettings.SelectedIndex == cmbSettings.Items.Count - 1)
            {
                tip.Show("select an existing profile or create new", cmbSettings, new Point(-10, -35), 5000);
                return false;
            }

            else return true;
        }

        private void btnRunEVE_Click(object sender, EventArgs e)
        {
            if(CheckFields())
            {
                lockForm = true;
                object eveParameters = new string[] { txtUsername.Text, txtPassword.Text, (string)cmbSettings.SelectedItem };
                ParameterizedThreadStart eveThreadStart = new ParameterizedThreadStart(RunEVE);
                Thread eve = new Thread(eveThreadStart);
                eve.Start(eveParameters);
                txtPassword.Text = string.Empty;
                btnRunEVE.Enabled = false;
                while (lockForm)
                {
                    Application.DoEvents();
                }
                if (characterChallenge)
                {
                    characterChallengeTab.Parent = this.tabs;
                    tabs.SelectedTab = characterChallengeTab;
                }
                btnRunEVE.Enabled = true;
            }
        }

        private void RunEVE(object parametersObj)
        {
            try
            {
                string[] parameters = (string[])parametersObj;
                string username = parameters[0];
                string password = parameters[1];
                string settingsProfile = parameters[2];
                string sso = launcher.GetSSO(username, password);
                if (sso == "cc")
                {
                    characterChallenge = true;
                    lockForm = false;
                    return;
                }
                launcher.RunEVE(sso, settingsProfile);
                lockForm = false;
            }
            catch (Exception ex)
            {
                lockForm = false;
                if (this.InvokeRequired)
                {
                    this.Invoke(action, ex);
                }
                else action(ex);
            }
        }

        private void btnSaveProfile_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckFields())
                {
                    if (diaSaveKey.ShowDialog(this) == DialogResult.OK)
                    {
                        DPAPI.CreateKeyFile(diaSaveKey.FileName, txtUsername.Text, txtPassword.Text, (string)cmbSettings.SelectedItem);
                    }
                }
            }
            catch (Exception ex)
            {
                if (this.InvokeRequired)
                {
                    this.Invoke(action, ex);
                }
                else action(ex);
            }
            finally
            {
                txtUsername.Text = string.Empty;
                txtPassword.Text = string.Empty;
            }
        }

        private void btnCCOk_Click(object sender, EventArgs e)
        {
            if (CheckCharacterNameField())
            {
                lblCCFailed.Visible = false;
                btnCCOk.Enabled = false;
                bool result = launcher.PerformCharacterChallengeRequest(txtUsername.Text, txtCharacterName.Text);
                txtCharacterName.Text = string.Empty;
                btnCCOk.Enabled = true;
                if (result)
                {
                    tabs.SelectedTab = mainTab;
                    characterChallengeTab.Parent = null;
                }
                else
                {
                    lblCCFailed.Visible = true;
                }
            }
        }
    }
}
