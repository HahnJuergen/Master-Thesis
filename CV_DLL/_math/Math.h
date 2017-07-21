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

	template <typename _T>
	inline _T const distance(cv::Point const & p, cv::Point const & q)
	{
		return vector_norm<_T>(q - p);
	}

	template <typename _T>
	inline float const cross(_T const & p, _T const & q)
	{
		return p.x * q.y - p.y * q.x;
	}

	template <typename _T>
	inline bool const intersection(_T & t, _T const & p, _T const & q, _T const & r, _T const & s)
	{	
		_T const x = r - p;
		_T const d1 = q - p;
		_T const d2 = s - r;

		float cross_d1_d2 = cross(d1, d2);

		if (cross_d1_d2 < 1e-8) 
			return false;

		float t1 = cross(x, d2) / cross_d1_d2;

		t = (_T) (p + d1 * t1);

		return true;
	}
private:


protected:
	Math() {}
};

#endif // CV_PROCESSING_API_MATH_H_