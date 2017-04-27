//  
// Copyright (c) 2017 Vulcan, Inc. All rights reserved.  
// Licensed under the Apache 2.0 license. See LICENSE file in the project root for full license information.
//

using System;

namespace HoloLensCameraStream
{
    public struct CameraParameters
    {
        public CapturePixelFormat pixelFormat;

        public int cameraResolutionHeight;

        public int cameraResolutionWidth;

        public int frameRate;

		public bool flip;

		public float hologramOpacity;
		public bool enableHolograms
		{
			get {	return hologramOpacity > 0.0f; }
			set {	hologramOpacity = value ? 1.0f : 0.0f; }
		}


        public bool enableRecordingIndicator;

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
        { throw new NotImplementedException(); }
    }
}
