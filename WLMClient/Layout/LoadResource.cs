using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using System.Net;
using System.IO;

using WLMData.Enums;
using WLMClient.UI.Data.Enums;

namespace WLMClient.Layout
{
    class LoadResource
    {
        public static CroppedBitmap GetSmallIconFromStatus(UserStatus status)
        {
            int smallIconsWidth = 0;

            if (status == UserStatus.Offline)
            {
                smallIconsWidth = Resource.Images.Attributes.USER_LIST_STATUS_OFFLINE;
            }

            if (status == UserStatus.Busy)
            {
                smallIconsWidth = Resource.Images.Attributes.USER_LIST_STATUS_BUSY;
            }

            if (status == UserStatus.Away)
            {
                smallIconsWidth = Resource.Images.Attributes.USER_LIST_STATUS_AWAY;
            }

            if (status == UserStatus.Available)
            {
                smallIconsWidth = Resource.Images.Attributes.USER_LIST_STATUS_AVAILABLE;
            }

            return new CroppedBitmap(Images.BITMAP_WINDOW_SMALL_ICONS, new Int32Rect(smallIconsWidth, 0, Resource.Images.Attributes.USER_LIST_STATUS_WIDTH, Resource.Images.Attributes.USER_LIST_STATUS_HEIGHT));
        }

        public static CroppedBitmap GetAvatarFrameFromStatus(UserStatus status, AvatarSize size)
        {
            int avatarFrameWidth = 0;

            if (status == UserStatus.Offline & size == AvatarSize.Big)
            {
                avatarFrameWidth = Resource.Images.Attributes.AVATAR_FRAME_STATUS_OFFLINE;
            }

            if (status == UserStatus.Busy & size == AvatarSize.Big)
            {
                avatarFrameWidth = Resource.Images.Attributes.AVATAR_FRAME_STATUS_BUSY;
            }

            if (status == UserStatus.Away & size == AvatarSize.Big)
            {
                avatarFrameWidth = Resource.Images.Attributes.AVATAR_FRAME_STATUS_AWAY;
            }

            if (status == UserStatus.Available & size == AvatarSize.Big)
            {
                avatarFrameWidth = Resource.Images.Attributes.AVATAR_FRAME_STATUS_AVAILABLE;
            }

            if (status == UserStatus.Offline & size == AvatarSize.Small)
            {
                avatarFrameWidth = Resource.Images.Attributes.AVATAR_FRAME_SMALL_STATUS_OFFLINE;
            }

            if (status == UserStatus.Busy & size == AvatarSize.Small)
            {
                avatarFrameWidth = Resource.Images.Attributes.AVATAR_FRAME_SMALL_STATUS_BUSY;
            }

            if (status == UserStatus.Away & size == AvatarSize.Small)
            {
                avatarFrameWidth = Resource.Images.Attributes.AVATAR_FRAME_SMALL_STATUS_AWAY;
            }

            if (status == UserStatus.Available & size == AvatarSize.Small)
            {
                avatarFrameWidth = Resource.Images.Attributes.AVATAR_FRAME_SMALL_STATUS_AVAILABLE;
            }

            if (size == AvatarSize.Big)
            {
                return new CroppedBitmap(Images.BITMAP_AVATAR_FRAME, new Int32Rect(avatarFrameWidth, 0, Resource.Images.Attributes.AVATAR_FRAME_WIDTH, Resource.Images.Attributes.AVATAR_FRAME_HEIGHT));
            }

            if (size == AvatarSize.Small)
            {
                return new CroppedBitmap(Images.BITMAP_AVATAR_FRAME, new Int32Rect(avatarFrameWidth, 0, Resource.Images.Attributes.AVATAR_FRAME_SMALL_WIDTH, Resource.Images.Attributes.AVATAR_FRAME_SMALL_HEIGHT));
            }

            return null;
        }

        public static CroppedBitmap GetDefaultAvatarImage()
        {
            return new CroppedBitmap(Images.BITMAP_AVATAR_FRAME, new Int32Rect(Resource.Images.Attributes.AVATAR_DEFAULT_IMAGE, 0, Resource.Images.Attributes.AVATAR_CHAT_SIZE_WIDTH, Resource.Images.Attributes.AVATAR_CHAT_SIZE_HEIGHT));
        }

        public static BitmapFrame Resize(BitmapSource image, int width, int height, BitmapScalingMode scalingMode)
        {
            DrawingGroup drawingGroup = new DrawingGroup();
            RenderOptions.SetBitmapScalingMode(drawingGroup, scalingMode);
            drawingGroup.Children.Add(new ImageDrawing(image, new Rect(0, 0, width, height)));

            DrawingVisual targetVisual = new DrawingVisual();

            DrawingContext targetContext = targetVisual.RenderOpen();
            targetContext.DrawDrawing(drawingGroup);

            RenderTargetBitmap target = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Default);
            targetContext.Close();
            target.Render(targetVisual);

            BitmapFrame targetFrame = BitmapFrame.Create(target);

            return targetFrame;
        }

        public static CroppedBitmap GetEmoticon(string emoticon)
        {
            return new CroppedBitmap(Images.BITMAP_EMOTICONS,
                new Int32Rect(Resource.Images.Emoticons.INDEX_IN_IMAGE[emoticon] * Resource.Images.Attributes.EMOTICON_WIDTH - Resource.Images.Attributes.EMOTICON_WIDTH,
                    0, Resource.Images.Attributes.EMOTICON_WIDTH, Resource.Images.Attributes.EMOTICON_HEIGHT));
        }

        public static BitmapImage GetAvatar(string avatar)
        {
            try
            {
                WebClient wc = new WebClient();
                var bytes = wc.DownloadData(avatar.Trim());

                MemoryStream ms = new MemoryStream(bytes);

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = ms;
                bitmap.EndInit();

                return bitmap;
            }
            catch
            {

            }

            return null;
        }

        public static CroppedBitmap chatWindowButtonSmileys(ButtonState state)
        {
            if (state == ButtonState.None)
            {
                return new CroppedBitmap(Images.BITMAP_CHAT_WINDOW_BUTTONS,
                    new Int32Rect(0, 0, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_WIDTH, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_HEIGHT));
            }

            if (state == ButtonState.Hover)
            {
                return new CroppedBitmap(Images.BITMAP_CHAT_WINDOW_BUTTONS,
                    new Int32Rect(Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_WIDTH, 0, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_WIDTH, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_HEIGHT));
            }

            if (state == ButtonState.Pressed)
            {
                return new CroppedBitmap(Images.BITMAP_CHAT_WINDOW_BUTTONS,
                    new Int32Rect(Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_WIDTH * 2, 0, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_WIDTH, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_HEIGHT));
            }

            return new CroppedBitmap(Images.BITMAP_CHAT_WINDOW_BUTTONS,
                new Int32Rect(0, 0, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_WIDTH, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_HEIGHT));
        }

        public static CroppedBitmap chatWindowButtonNudge(ButtonState state)
        {
            if (state == ButtonState.None)
            {
                return new CroppedBitmap(Images.BITMAP_CHAT_WINDOW_BUTTONS,
                    new Int32Rect(0, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_HEIGHT, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_WIDTH, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_HEIGHT));
            }

            if (state == ButtonState.Hover)
            {
                return new CroppedBitmap(Images.BITMAP_CHAT_WINDOW_BUTTONS,
                    new Int32Rect(Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_WIDTH, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_HEIGHT, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_WIDTH, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_HEIGHT));
            }

            if (state == ButtonState.Pressed)
            {
                return new CroppedBitmap(Images.BITMAP_CHAT_WINDOW_BUTTONS,
                    new Int32Rect(Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_WIDTH * 2, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_HEIGHT, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_WIDTH, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_HEIGHT));
            }

            return new CroppedBitmap(Images.BITMAP_CHAT_WINDOW_BUTTONS,
                new Int32Rect(0, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_HEIGHT, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_WIDTH, Resource.Images.Attributes.CHAT_WINDOW_BUTTONS_HEIGHT));
        }

        public static System.Windows.Controls.Image getParagraphRectangle()
        {
            System.Windows.Controls.Image image = new System.Windows.Controls.Image();

            image.Source = Images.BITMAP_CHAT_PARAGRAPH_RECTANGLE;
            image.SnapsToDevicePixels = true;
            image.Width = 3;
            image.Height = 3;
            image.Stretch = Stretch.Fill;

            RenderOptions.SetBitmapScalingMode(image, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(image, EdgeMode.Aliased);

            return image;
        }
    }
}
