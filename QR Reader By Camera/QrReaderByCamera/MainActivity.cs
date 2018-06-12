using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Vision;
using Android.Gms.Vision.Barcodes;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V7.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using System;
using static Android.Gms.Vision.Detector;

namespace QrReaderByCamera
{
    [Activity(Label = "QrReaderByCamera", MainLauncher = true , Theme ="@style/Theme.AppCompat.Light.NoActionBar")]
    public class MainActivity : AppCompatActivity , ISurfaceHolderCallback, IProcessor
    {
        SurfaceView surfaceView;
        TextView txtResult;
        BarcodeDetector barcodeDetector;
        CameraSource cameraSource;
        const int RequestCameraPermisionID = 1001;

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            switch (requestCode)
            {
                case RequestCameraPermisionID:
                    {
                        if(grantResults[0] == Permission.Granted)
                        {
                            if (ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
                            {
                                //Request Permision
                                ActivityCompat.RequestPermissions(this, new string[]
                                {
                    Manifest.Permission.Camera
                                }, RequestCameraPermisionID);
                                return;
                            }
                            try
                            {
                                cameraSource.Start(surfaceView.Holder);
                            }
                            catch (InvalidOperationException)
                            {

                            }
                        }
                    }
                    break;
            }
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            surfaceView = FindViewById<SurfaceView>(Resource.Id.cameraView);
            txtResult = FindViewById<TextView>(Resource.Id.txtResult);

            Bitmap bitMap = BitmapFactory.DecodeResource(ApplicationContext
            .Resources, Resource.Drawable.qrcode);

            barcodeDetector = new BarcodeDetector.Builder(this)
                .SetBarcodeFormats(BarcodeFormat.QrCode)
                .Build();
            cameraSource = new CameraSource
                .Builder(this, barcodeDetector)
                .SetRequestedPreviewSize(640, 480)
                .Build();
            surfaceView.Holder.AddCallback(this);
            barcodeDetector.SetProcessor(this);
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            if(ActivityCompat.CheckSelfPermission(ApplicationContext, Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
            {
                //Request Permision
                ActivityCompat.RequestPermissions(this, new string[]
                {
                    Manifest.Permission.Camera
                }, RequestCameraPermisionID);
                return;
            }
            try
            {
                cameraSource.Start(surfaceView.Holder);
            }
            catch (InvalidOperationException)
            {

            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            cameraSource.Stop();
        }

        public void ReceiveDetections(Detections detections)
        {
            SparseArray qrcodes = detections.DetectedItems;
            if (qrcodes.Size() != 0)
            {
                txtResult.Post(() => {
                    Vibrator vibrator = (Vibrator)GetSystemService(Context.VibratorService);
                    vibrator.Vibrate(1000);
                    txtResult.Text = ((Barcode)qrcodes.ValueAt(0)).RawValue;
                });
            }
        }

        public void Release()
        {
            throw new NotImplementedException();
        }
    }
}

