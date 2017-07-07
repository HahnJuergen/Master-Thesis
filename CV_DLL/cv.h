#pragma once

#ifndef CV_PROCESSING_API_CV_H_
#define CV_PROCESSING_API_CV_H_

#include "_processing\Processing.h"
#include "_utility\utility.h"

#include <opencv2\aruco.hpp>
#include <opencv2\core.hpp>
#include <opencv2\imgproc.hpp>
#include <opencv2\calib3d.hpp>
#include <opencv2\core\hal\hal.hpp>
#include <string>>
#include "_math/Math.h"
#include "_constants\Constants.h"

#define CV_PROCESSING_API __declspec(dllexport) 

typedef void(__stdcall * print_function)(const char *);

extern "C"
{
	CV_PROCESSING_API int initialize(int const _image_width, int const _image_height);
	CV_PROCESSING_API int process(unsigned char * _image_data, float * _corners, float * _out_rvec, float * _out_tvec);
	CV_PROCESSING_API int set_debug_print(print_function _function_pointer);
	CV_PROCESSING_API int is_lib_initialized();
}

#endif // CV_PROCESSING_API_CV_H_
