using Microsoft.Maui.Handlers;
using OS.OBDII.Controls;
using OS.OBDII.Interfaces;
using Microsoft.Maui.Platform;
using ListViewRenderer = Microsoft.Maui.Controls.Handlers.Compatibility.ListViewRenderer;
using Microsoft.Maui.Controls.Platform;
using Microsoft.UI.Xaml;
using OS.OBDII.Models;
using Microsoft.Maui.Controls.Compatibility.Platform.UWP;

namespace OS.OBDII.PartialClasses;

public partial class OSListViewHandler : ListViewRenderer
{


    public OSListViewHandler()
    {
    }

    protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
    {
        base.OnElementChanged(e as ElementChangedEventArgs<ListView>);

        if (e.OldElement != null)
        {
            if (Control == null) return;
            // unsubscribe
            Control.DoubleTapped -= onDoubleTapped;
        }

        if (e.NewElement != null)
        {
            if (Control == null) return;
            Control.DoubleTapped += onDoubleTapped;
        }

    }

    private void onDoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
    {
        //var lv = ((OSListView)Element);

        //var itemList = lv.Items.ToList();
        //var itemHeight = lv.Height / lv.Items.Count();
        //int idx = Convert.ToInt32(((2* e.GetPosition(this).Y- (itemHeight*1.5)) /itemHeight) ) +1;
        //if (idx > itemList.Count() - 1 || idx < 0) return;

        //((OSListView)Element).NotifyItemSelected(itemList[idx],  idx);


        ((OSListView)Element).NotifyItemSelected(null, 0);

    }


    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Control.DoubleTapped -= onDoubleTapped;
        }

        base.Dispose(disposing);
    }

}
