using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Bandit.Behaviors
{
    public class PasswordBoxBehavior : Behavior<PasswordBox>
    {
        // BoundPassword
        public SecureString BoundPassword
        {
            get
            {
                return (SecureString)GetValue(BoundPasswordProperty);
            }
            set
            {
                SetValue(BoundPasswordProperty, value);
            }
        }

        public static readonly DependencyProperty BoundPasswordProperty = DependencyProperty.Register("BoundPassword",
            typeof(SecureString), typeof(PasswordBoxBehavior), new FrameworkPropertyMetadata(OnBoundPasswordChanged));

        protected override void OnAttached()
        {
            AssociatedObject.PasswordChanged += AssociatedObjectOnPasswordChanged;
            base.OnAttached();
        }

        /// <summary>
        /// Link up the intermediate SecureString (BoundPassword) to the UI instance
        /// </summary>
        private void AssociatedObjectOnPasswordChanged(object s, RoutedEventArgs e)
        {
            BoundPassword = AssociatedObject.SecurePassword;
        }

        /// <summary>
        /// Reacts to password reset on viewmodel (ViewModel.Password = new SecureString())
        /// </summary>
        private static void OnBoundPasswordChanged(object s, DependencyPropertyChangedEventArgs e)
        {
            var box = ((PasswordBoxBehavior)s).AssociatedObject;

            if (box != null)
            {
                if (((SecureString)e.NewValue).Length == 0)
                {
                    box.Password = string.Empty;
                }
            }
        }
    }
}
