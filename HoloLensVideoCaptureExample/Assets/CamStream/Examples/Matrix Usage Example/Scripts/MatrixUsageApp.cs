using UnityEngine;
using UnityEngine.VR.WSA;

using System;

using HoloLensCameraStream;

public class MatrixUsageApp : MonoBehaviour
{
    byte[] _latestImageBytes;
    HoloLensCameraStream.Resolution _resolution;

    //"Injected" objects.
    VideoPanel _videoPanelUI;
    VideoCapture _videoCapture;
    IndicatorDisplay _targetIndicator;

    IntPtr _spatialCoordinateSystemPtr;

    void Start()
    {
        //Fetch a pointer to Unity's spatial coordinate system if you need pixel mapping
        _spatialCoordinateSystemPtr = WorldManager.GetNativeISpatialCoordinateSystemPtr();

        //Call this in Start() to ensure that the CameraStreamHelper is already "Awake".
        CameraStreamHelper.Instance.GetVideoCaptureAsync(OnVideoCaptureCreated);
        //You could also do this "shortcut":
        //CameraStreamManager.Instance.GetVideoCaptureAsync(v => videoCapture = v);

        _videoPanelUI = GameObject.FindObjectOfType<VideoPanel>();
        _targetIndicator = GameObject.FindObjectOfType<IndicatorDisplay>();
    }

    private void OnDestroy()
    {
        if (_videoCapture != null)
        {
            _videoCapture.FrameSampleAcquired -= OnFrameSampleAcquired;
            _videoCapture.Dispose();
        }
    }

    void OnVideoCaptureCreated(VideoCapture videoCapture)
    {
        if (videoCapture == null)
        {
            Debug.LogError("Did not find a video capture object. You may not be using the HoloLens.");
            return;
        }

        this._videoCapture = videoCapture;

        //Request the spatial coordinate ptr if you want fetch the camera and set it if you need to 
        CameraStreamHelper.Instance.SetNativeISpatialCoordinateSystemPtr(_spatialCoordinateSystemPtr);

        _resolution = CameraStreamHelper.Instance.GetLowestResolution();
        float frameRate = CameraStreamHelper.Instance.GetHighestFrameRate(_resolution);
        videoCapture.FrameSampleAcquired += OnFrameSampleAcquired;

        //You don't need to set all of these params.
        //I'm just adding them to show you that they exist.
        CameraParameters cameraParams = new CameraParameters();
        cameraParams.cameraResolutionHeight = _resolution.height;
        cameraParams.cameraResolutionWidth = _resolution.width;
        cameraParams.frameRate = Mathf.RoundToInt(frameRate);
        cameraParams.pixelFormat = CapturePixelFormat.BGRA32;
        cameraParams.rotateImage180Degrees = false; //If your image is upside down, remove this line.
        cameraParams.enableHolograms = false;

        UnityEngine.WSA.Application.InvokeOnAppThread(() => { _videoPanelUI.SetResolution(_resolution.width, _resolution.height); }, false);

        videoCapture.StartVideoModeAsync(cameraParams, OnVideoModeStarted);
    }

    void OnVideoModeStarted(VideoCaptureResult result)
    {
        if (result.success == false)
        {
            Debug.LogWarning("Could not start video mode.");
            return;
        }

        Debug.Log("Video capture started.");
    }

    // Everything above here is boilerplate from the VideoPanelApp.cs project
    void OnFrameSampleAcquired(VideoCaptureSample sample)
    {
        //When copying the bytes out of the buffer, you must supply a byte[] that is appropriately sized.
        //You can reuse this byte[] until you need to resize it (for whatever reason).
        if (_latestImageBytes == null || _latestImageBytes.Length < sample.dataLength)
        {
            _latestImageBytes = new byte[sample.dataLength];
        }
        sample.CopyRawImageDataIntoBuffer(_latestImageBytes);

        //If you need to get the cameraToWorld matrix for purposes of compositing you can do it like this
        float[] cameraToWorldMatrixAsFloat;
        if (sample.TryGetCameraToWorldMatrix(out cameraToWorldMatrixAsFloat) == false)
        {
            return;
        }

        //If you need to get the projection matrix for purposes of compositing you can do it like this
        float[] projectionMatrixAsFloat;
        if (sample.TryGetProjectionMatrix(out projectionMatrixAsFloat) == false)
        {
            return;
        }

        // Right now we pass things across the pipe as a float array then convert them back into UnityEngine.Matrix using a utility method
        Matrix4x4 cameraToWorldMatrix = LocatableCameraUtils.ConvertFloatArrayToMatrix4x4(cameraToWorldMatrixAsFloat);
        Matrix4x4 projectionMatrix = LocatableCameraUtils.ConvertFloatArrayToMatrix4x4(projectionMatrixAsFloat);

        //This is where we actually use the image data
        UnityEngine.WSA.Application.InvokeOnAppThread(() =>
        {
            _videoPanelUI.SetBytes(_latestImageBytes);

            Vector3 inverseNormal = LocatableCameraUtils.GetNormalOfPose(cameraToWorldMatrix);
            Vector3 imagePosition = cameraToWorldMatrix.MultiplyPoint(Vector3.zero);

            // Throw out an indicator in the composite space 2 meters in front of us using the corresponding view matrices
            float distanceToMarker = 2f;
            Vector3 pointOnFaceBoxPlane = imagePosition - inverseNormal * distanceToMarker;
            Plane surfacePlane = new Plane(inverseNormal, pointOnFaceBoxPlane);

            Vector2 targetPoint = new Vector2(_resolution.width * 0.5f, _resolution.height * 0.5f);
            Vector3 mdPoint = LocatableCameraUtils.PixelCoordToWorldCoord(cameraToWorldMatrix, projectionMatrix, _resolution, targetPoint, surfacePlane);

            _targetIndicator.SetPosition(mdPoint);
            _targetIndicator.SetText("P");

            _videoPanelUI.gameObject.SetActive(false);
        }, false);
    }
}