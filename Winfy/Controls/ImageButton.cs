using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Winfy.Controls {
    public sealed class ImageButton : Button {

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof (ImageSource), typeof (ImageButton), new PropertyMetadata(default(ImageSource)));

        public static readonly DependencyProperty ImageSourceNormalProperty =
            DependencyProperty.Register("ImageSourceNormal", typeof (ImageSource), typeof (ImageButton), new PropertyMetadata(default(ImageSource)));

        public static readonly DependencyProperty ImageSourcePressedProperty =
            DependencyProperty.Register("ImageSourcePressed", typeof (ImageSource), typeof (ImageButton), new PropertyMetadata(default(ImageSource)));

        public ImageSource ImageSourcePressed {
            get { return (ImageSource) GetValue(ImageSourcePressedProperty); }
            set { SetValue(ImageSourcePressedProperty, value); }
        }

        public ImageSource ImageSourceNormal {
            get { return (ImageSource) GetValue(ImageSourceNormalProperty); }
            set { SetValue(ImageSourceNormalProperty, value); }
        }

        public ImageSource ImageSource {
            get { return (ImageSource) GetValue(ImageSourceProperty); }
            set { SetValue(ImageSourceProperty, value); }
        }

        protected override void OnMouseDown(System.Windows.Input.MouseButtonEventArgs e) {
            base.OnMouseDown(e);
            ImageSource = ImageSourcePressed;
        }

        protected override void OnMouseUp(System.Windows.Input.MouseButtonEventArgs e) {
            base.OnMouseUp(e);
            ImageSource = ImageSourceNormal;
        }
    }
}
