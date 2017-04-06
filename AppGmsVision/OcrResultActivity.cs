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

namespace AppGmsVision
{
    [Activity(Label = "OcrResultActivity")]
    public class OcrResultActivity : Activity
    {
        public const string INTENTKEY_WORDS = "OcrResultActivity_Words";
        string[] _words;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.OcrResult);

            _words = Intent.GetStringArrayExtra(INTENTKEY_WORDS);

            var listview = FindViewById<ListView>(Resource.Id.listViewOcrWords);
            var arrayAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleExpandableListItem1, _words);
            listview.Adapter = arrayAdapter;

            RegisterForContextMenu(listview);
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            switch(v.Id)
            {
                case Resource.Id.listViewOcrWords:
                    MenuInflater.Inflate(Resource.Menu.menuOcrResult, menu);
                    menu.SetHeaderTitle("“ü—Í‚Æ‚µ‚ÄŽg—p");
                    break;
            }
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            int? textboxId;
            switch (item.ItemId)
            {
                case Resource.Id.menuocrresult_input1:
                    textboxId = Resource.Id.editTextInput1;
                    break;
                case Resource.Id.menuocrresult_input2:
                    textboxId = Resource.Id.editTextInput2;
                    break;
                default:
                    return base.OnContextItemSelected(item);
            }

            var info = item.MenuInfo as AdapterView.AdapterContextMenuInfo;
            if (info == null) { return false; }

            var word = _words.ElementAtOrDefault(info.Position);

            var text = FindViewById<EditText>(textboxId.Value);
            text.Text = word;

            return true;
        }
    }
}