/*
 * Copyright © 2012-2013 Nokia Corporation. All rights reserved.
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation. 
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners. 
 * See LICENSE.TXT for license information.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FastFoodFinder.Resources;
using System.Xml.Linq;
using Microsoft.Phone.Tasks;

namespace FastFoodFinder
{
    public partial class AboutPage : PhoneApplicationPage
    {
        public AboutPage()
        {
            InitializeComponent();
            UpdateVersionString();
        }

        private void UpdateVersionString()
        {
            string appVersion = XDocument.Load("WMAppManifest.xml").Root.Element("App").Attribute("Version").Value;
            VersionText.Text = AppResources.AboutPageVersionText + appVersion;
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.To = "FastFoodFinder@outlook.com";

            emailComposeTask.Show();
        }
    }
}