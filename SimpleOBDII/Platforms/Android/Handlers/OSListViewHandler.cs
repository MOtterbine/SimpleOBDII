using Microsoft.Maui.Handlers;
using OS.OBDII.Controls;
using Android.Views;
using Google.Android.Material.Button;
using OS.OBDII.Interfaces;
using Microsoft.Maui.Platform;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using ListViewRenderer = Microsoft.Maui.Controls.Handlers.Compatibility.ListViewRenderer;
using Android.Content;
using Microsoft.Maui.Controls.Platform;
using static Android.Widget.AdapterView;

namespace OS.OBDII;

public partial class OSListViewHandler : ListViewRenderer
{


    public OSListViewHandler(Android.Content.Context context) : base(context)
    {
    }

    protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
    {
        base.OnElementChanged(e);

        if (e.OldElement != null)
        {
            if (Control == null) return;
            // unsubscribe
            //Control.ItemSelected -= OnItemSelected;
            //Control.ItemClick -= OnItemClick;
            Control.ItemLongClick -= OnItemLongClick;
            //Control.SetOnTouchListener(null);
            //Control.SetOnLongClickListener(null);
        }

        if (e.NewElement != null)
        {
            if (Control == null) return;

            //Control.ItemSelected += OnItemSelected;
            //    Control.ite
            //Control.ItemClick += OnItemClick;
            Control.ItemLongClick += OnItemLongClick;
            //Control.SetOnTouchListener(ButtonTouchListener.Instance.Value);
            //Control.LongClickable = true;
            //Control.SetOnLongClickListener(ButtonLongClickListener.Instance.Value);

        }

    }

    private void OnItemSelected(object sender, ItemSelectedEventArgs e)
    {
        // e.ob
        var itemList = ((OSListView)Element).Items.ToList();
        // var idx = e.Position;

        ((OSListView)Element).NotifyItemSelected(itemList[e.Position], e.Position);
        // ((OS.OBD2.Views.OSListView)Element).NotifyItemLongClicked(itemList[idx], idx-1);
    }

    private void OnItemLongClick(object sender, ItemLongClickEventArgs e)
    {
        var itemList = ((OSListView)Element).Items.ToList();
        var idx = e.Position;

        ((OSListView)Element).NotifyItemLongClicked(itemList[idx-1], idx- 1);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Control.ItemLongClick -= OnItemLongClick;
        }

        base.Dispose(disposing);
    }

}
