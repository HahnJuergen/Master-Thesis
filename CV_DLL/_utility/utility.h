#pragma once

#ifndef CV_PROCESSING_API_UTILITY_H_
#define CV_PROCESSING_API_UTILITY_H_

#include <opencv2\core.hpp>
#include <opencv2\imgproc.hpp>

namespace Utility
{
	enum Color
	{
		RED, GREEN, BLUE, YELLOW
	};

	inline cv::Scalar _drawing_color(Color _color)
	{
		switch (_color)
		{
			case RED: return cv::Scalar(255, 0, 0);
			case GREEN: return cv::Scalar(0, 255, 0);
			case BLUE: return cv::Scalar(0, 0, 255);
			case YELLOW: return cv::Scalar(255, 255, 0);
		}
	}

	inline void draw_line(cv::Mat & _image, cv::Point const & _p, cv::Point const & _q, Color const & _color)
	{
		cv::line(_image, _p, _q, _drawing_color(_color), 2);
	}

	inline void draw_circle(cv::Mat & _image, cv::Point const & _p, int const _radius, Color const & _color)
	{
		cv::circle(_image, _p, _radius, _drawing_color(_color), CV_FILLED);
	}

	template <typename _T>
	inline void draw_polygon(cv::Mat & _image, std::vector<_T> const & _polygon, Color const & _color)
	{
		for (uint32_t i = 0; i < _polygon.size(); i++)
			draw_line(_image, _polygon[i], _polygon[(i + 1) % _polygon.size()], _color);
	}
}

#endif //CV_PROCESSING_API_UTILITY_H_
