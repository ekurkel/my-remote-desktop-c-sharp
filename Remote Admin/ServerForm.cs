﻿using System;
using System.Drawing;
using System.Net;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Remote_Admin.Model;
using MaterialSkin.Controls;
using MaterialSkin;
using MaterialSkin.Animations;

namespace Remote_Admin
{
    public partial class ServerForm : MaterialForm
    {
        public Server server { get; private set; }
        
        public ServerForm()
        {
            InitializeComponent();

            server = new Server();
            server.RemoteComputersListHasChanged += UpdateRemoteComputers;


            MaterialSkin.MaterialSkinManager skinManager = MaterialSkin.MaterialSkinManager.Instance;
            skinManager.AddFormToManage(this);
            skinManager.Theme = MaterialSkin.MaterialSkinManager.Themes.LIGHT;
            skinManager.ColorScheme = new MaterialSkin.ColorScheme(MaterialSkin.Primary.Blue800, MaterialSkin.Primary.BlueGrey700, MaterialSkin.Primary.Blue500, MaterialSkin.Accent.Orange700, MaterialSkin.TextShade.WHITE);


            Label7.Text = Environment.MachineName;
            Label6.Text = Dns.GetHostByName(Environment.MachineName).AddressList[0].ToString();
        }

        private void UpdateRemoteComputers()
        {
            if (this.InvokeRequired)
                this.Invoke(new MethodInvoker(delegate ()
                {
                    listViewClients.Items.Clear();
                    for(int i = 0; i < server.RemoteComputers.Count; i++)
                    {
                        listViewClients.Items.Add(new ListViewItem( new string[] { i.ToString(), server.RemoteComputers[i].ComputerName, server.RemoteComputers[i].ComputerUser, server.RemoteComputers[i].ClientIP } ));
                    }
                   
                }));
        }
        

        private void closeAllConnectionsButton_Click(object sender, EventArgs e)
        {
            server.CloseAllConnections();
        }

        private void remoteDesctopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewClients.SelectedItems.Count < 1)
            {
                MessageBox.Show("You have to select a client in order to access this function!",
                   "ERROR : Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                var rdf = new RemoteDesctopForm(server.RemoteComputers[listViewClients.Items.IndexOf(listViewClients.SelectedItems[0])]);
                rdf.ShowDialog();
            }
        }

        private void sendFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewClients.SelectedItems.Count < 1)
            {
                MessageBox.Show("You have to select a client in order to access this function!",
                    "ERROR : Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                try
                {
                    Commands.SendFile(server.RemoteComputers[listViewClients.FocusedItem.Index].clientSocket);
                }
                catch { }
            }
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listViewClients.SelectedItems.Count < 1)
            {
                MessageBox.Show("You have to select a client in order to access this function!",
                    "ERROR : Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            { 
                server.CloseConnections(listViewClients.FocusedItem.Index);
            }
        }
    }
}
