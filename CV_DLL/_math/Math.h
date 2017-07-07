#pragma once

#ifndef CV_PROCESSING_API_MATH_H_
#define CV_PROCESSING_API_MATH_H_

#include "pch.h"

#include <opencv2/core.hpp>
#include <opencv2\imgproc.hpp>
#include <cmath>

class Math : public Singleton<Math>
{
	friend class Singleton<Math>;

public:
	~Math() {}

	template<typename _T>
	inline _T const angle_degrees(cv::Point const & _p, cv::Point const & _q)
	{
		return (_T)(std::acos(_p.dot(_q) / (vector_norm<_T>(_p) * vector_norm<_T>(_q))) * (cnst::math::RIGHT_ANGLE * 2) / cnst::math::PI);
	}

	template <typename _T>
	inline _T const vector_norm(cv::Point const & _p)
	{
		return (_T)std::sqrt(_p.x * _p.x + _p.y * _p.y);
	}

private:


protected:
	Math() {}
};

#endif // CV_PROCESSING_API_MATH_H_