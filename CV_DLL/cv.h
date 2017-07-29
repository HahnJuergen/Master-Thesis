#pragma once

#ifndef CV_PROCESSING_API_CV_H_
#define CV_PROCESSING_API_CV_H_

#include "_processing\Processing.h"
#include "_utility\utility.h"

#include <opencv2\core.hpp>
#include <opencv2\imgproc.hpp>
#include <opencv2\calib3d.hpp>
#include <string>>
#include "_math/Math.h"
#include "_constants\Constants.h"

#define CV_PROCESSING_API __declspec(dllexport) 

typedef void(__stdcall * print_function)(const char *);
typedef unsigned char byte;

extern "C"
{
	CV_PROCESSING_API uint32_t initialize(uint32_t const _image_width, uint32_t const _image_height);
	CV_PROCESSING_API uint32_t process(byte * _image_data, float * _corners, float * _out_rvec, float * _out_tvec, uint32_t const _accuracy);
	CV_PROCESSING_API uint32_t set_debug_print(print_function _function_pointer);
	CV_PROCESSING_API uint32_t is_lib_initialized();
}

#endif // CV_PROCESSING_API_CV_H_
