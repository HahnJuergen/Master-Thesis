#pragma once

#ifndef CV_PROCESSING_API_CONSTANTS_H_
#define CV_PROCESSING_API_CONSTANTS_H_

#pragma once

#include <opencv2\core.hpp>

static float const RIGHT_ANGLE = 90.0f;
static float const PI = CV_PI;
static float const REJECTION_ANGLE = 15.0f;
static float const ARC_LENGTH_MULTIPLICATOR = 0.1f;
static float const EPSILON_RADIUS = 0.01f;
static float const EPSILON_ANGLE = 0.01f;
static float const RAD2DEG = 57.29577f;

static cv::Scalar const LOWER_COLOR_INTERVAL_BOUND(60 - 30, 110, 110);
static cv::Scalar const UPPER_COLOR_INTERVAL_BOUND(60 + 30, 255, 255);

static cv::Size const BLURRING_KERNEL_SIZE(11, 11);

static uint32_t const SUFFICIENT_NUMBER_CORNERS = 4;
static uint32_t const RHO = 1;
static uint32_t const THRESHOLD = 35;
static uint32_t const CHOOSE_OPTIMAL = 0;
static uint32_t const SIGMA_X = 0;
static uint32_t const SIGMA_Y = 0;
static uint32_t const CHANNELS = 3;

static double const THETA = PI / (RIGHT_ANGLE * 2);
static double const MIN_LENGTH_LINE = 20.0;
static double const MAX_GAP_LINE = 15.0;

#endif /* CV_PROCESSING_API_CONSTANTS_H_ */