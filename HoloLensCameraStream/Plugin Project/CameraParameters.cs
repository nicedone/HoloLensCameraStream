//  
// Copyright (c) 2017 Vulcan, Inc. All rights reserved.  
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
//

namespace HoloLensCameraStream
{
    /// <summary>
    /// When calling VideoCapture.StartPhotoModeAsync, you must pass in a CameraParameters object
    /// that contains the various settings that the web camera will use.
    /// </summary>
    public struct CameraParameters
    {
        /// <summary>
        /// The pixel format used to capture and record your image data.
        /// </summary>
        public CapturePixelFormat pixelFormat;

        /// <summary>
        /// The height of the image frame. Must be a valid option.
        /// Valid options can be obtained from VideoCapture.SupportedResolutions.
        /// </summary>
        public int cameraResolutionHeight;

        /// <summary>
        /// The width of the image frame. Must be a valid option.
        /// Valid options can be obtained from VideoCapture.SupportedResolutions.
        /// </summary>
        public int cameraResolutionWidth;

        /// <summary>
        /// The frames per second that the video will stream at. Must be supported based on the selected resolution.
        /// </summary>
        public int frameRate;

		/// <summary>
        /// Rotate image 180 degrees.
        /// </summary>
        public bool flip;

		/// <summary>
        /// Enable holograms if opacity > 0. (internally these are two options, but rolled into one)
		/// https://developer.microsoft.com/en-us/windows/mixed-reality/mixed_reality_capture_for_developers
        /// </summary>
        public float hologramOpacity;
		public bool enableHolograms
		{
			get {	return hologramOpacity > 0.0f; }
			set {	hologramOpacity = value ? 0.9f : 0.0f; }
		}


 		/// <summary>
        /// Enable Recording label (on by default)
		/// https://developer.microsoft.com/en-us/windows/mixed-reality/mixed_reality_capture_for_developers
        /// </summary>
        public bool enableRecordingIndicator;

		/// <summary>
        /// Enable Video stabilisation and set the buffer amount. Docs recommend 15.
		/// https://developer.microsoft.com/en-us/windows/mixed-reality/mixed_reality_capture_for_developers
        /// </summary>
 		public int videoStabilizationBufferSize;
		public bool enableVideoStabilization
		{
			get {	return videoStabilizationBufferSize > 0; }
			set {	videoStabilizationBufferSize = value ? 15 : 0; }
		}

        public CameraParameters(
            CapturePixelFormat pixelFormat = CapturePixelFormat.BGRA32,
            int cameraResolutionHeight = 720,
            int cameraResolutionWidth = 1280,
            int frameRate = 30,
			bool flip = true)
        {
            this.pixelFormat = pixelFormat;
            this.cameraResolutionHeight = cameraResolutionHeight;
            this.cameraResolutionWidth = cameraResolutionWidth;
            this.frameRate = frameRate;
			this.flip = flip;
			this.videoStabilizationBufferSize = 0;
			this.hologramOpacity = 0.0f;
			this.enableRecordingIndicator = true;
        }
    }
}
