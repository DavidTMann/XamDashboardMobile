using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MobileDashboard
{
    class dialog_LogIn : DialogFragment
    {
        MainActivity main = new MainActivity();

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);

            var view = inflater.Inflate(Resource.Layout.LogInPrompt, container, false);

            TextView userTxt = view.FindViewById<TextView>(Resource.Id.userNameTxtPrompt);
            TextView pwdTxt = view.FindViewById<TextView>(Resource.Id.passwordTextPrompt);
            TextView incorrectCred = view.FindViewById<TextView>(Resource.Id.incorrectPwdTextPrompt);

            Button signInBtn = view.FindViewById<Button>(Resource.Id.SignInBtnPrompt);

            userTxt.Text = MainActivity.userName;

            signInBtn.Click += delegate
            {
                bool valid = main.ValidateUserLogin(userTxt, pwdTxt, incorrectCred, true);

                if (valid)
                {
                    //Go to menu dashboard page                    
                    Intent menu = new Intent(this.Activity, typeof(MenuActivity));

                    StartActivity(menu);
                }
                else
                {
                    incorrectCred.Visibility = ViewStates.Visible;
                }                
            };           

            return view;
        }
    }
}