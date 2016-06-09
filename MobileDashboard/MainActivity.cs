using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace MobileDashboard
{
    [Activity(Label = "Dashboard Mobile", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {       

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);



            // Get our button from the layout resource,
            // and attach an event to it
            Button signInBtn = FindViewById<Button>(Resource.Id.SignInBtn);

            //username field
            TextView user = FindViewById<TextView>(Resource.Id.userNameTxt);
            user.SetSingleLine();
            //pwd field
            TextView pwd = FindViewById<TextView>(Resource.Id.passwordText);
            user.SetSingleLine();

            //incorrect credentials txt box
            var incorrectCredTxt = FindViewById<TextView>(Resource.Id.incorrectPwdText);

            signInBtn.Click += delegate
            {
                ValidateUser(user, pwd, incorrectCredTxt);
            };
        }

        private void ValidateUser(TextView user, TextView pwd, TextView incorrectCredTxt)
        {
            if (!(user.Text == "aylingw" && pwd.Text == "password") || user.Text == string.Empty && pwd.Text == string.Empty)
            {
                //Show error message
                incorrectCredTxt.Visibility = ViewStates.Visible;
            }
            else
            {
                //Go to menu dashboard page                    
                Intent menu = new Intent(this.ApplicationContext, typeof(MenuActivity));
                menu.PutExtra("user", user.Text);
                StartActivity(menu);
            }
        }
    }
}

