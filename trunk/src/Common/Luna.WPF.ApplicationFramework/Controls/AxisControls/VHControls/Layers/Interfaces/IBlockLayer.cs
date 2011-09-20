using System.Windows;
using System.Windows.Input;

namespace Luna.WPF.ApplicationFramework.Controls
{
    public interface IBlockGridLayer
    {
        void OnParentHitOutOfRangePrivate();

        void OnParentMouseDownPrivate(object sender, MouseButtonEventArgs e);

        void OnParentMouseUpPrivate(object sender, MouseButtonEventArgs e);

        void OnParentMouseMovePrivate(object sender, MouseEventArgs e);

        void OnParentMouseEnterPrivate(object sender, MouseEventArgs e);

        void OnParentMouseLeavePrivate(object sender, MouseEventArgs e);

        void OnParentKeyDownPrivate(object sender, KeyEventArgs e);

       

        void OnParentKeyEscPressPrivate(object sender, KeyEventArgs e);

        void OnParentKeyUpPrivate(object sender, KeyEventArgs e);

        void OnParentDragOverPrivate(DragEventArgs e);

        void OnParentDropPrivate(DragEventArgs e);

        void OnParentDragLeavePrivate(DragEventArgs e);

        void OnParentTextInputPrivate(TextCompositionEventArgs e);

        void OnParentMouseDoubleClickPrivate(object sender, MouseButtonEventArgs e);
        //BlockGridLayerContainer LayerContainer { get; set; }
    }
}
//:IServiceable