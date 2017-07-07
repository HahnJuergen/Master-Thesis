#pragma once

#ifndef CV_PROCESSING_API_CONSTANTS_H_
#define CV_PROCESSING_API_CONSTANTS_H_

#pragma once

#include <opencv2\core.hpp>

namespace cnst
{
	namespace math
	{
		static float RIGHT_ANGLE = 90.0f;
		static float PI = CV_PI;
	}

	namespace processing
	{
		static float REJECTION_ANGLE = 15.0f;

		static cv::Scalar LOWER_COLOR_INTERVAL_BOUND(60 - 35, 110, 110);
		static cv::Scalar UPPER_COLOR_INTERVAL_BOUND(60 + 35, 255, 255);

		static float ARC_LENGTH_MULTIPLICATOR = 0.025f;

		static int SUFFICIENT_NUMBER_CORNERS = 4;

		namespace hough
		{
			static int RHO = 1;
			static double THETA = cnst::math::PI / (cnst::math::RIGHT_ANGLE * 2);
			static int THRESHOLD = 35;
			static double MIN_LENGTH_LINE = 20.0;
			static double MAX_GAP_LINE = 15.0;
		}
	}

}
#endif /* CV_PROCESSING_API_CONSTANTS_H_ */