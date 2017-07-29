#pragma once

#ifndef CV_PROCESSING_API_MATH_H_
#define CV_PROCESSING_API_MATH_H_

#include "pch.h"

#include <opencv2/core.hpp>
#include <opencv2\imgproc.hpp>
#include <cmath>
#include <algorithm>

class Math : public Singleton<Math>
{
	friend class Singleton<Math>;

public:
	~Math() {}

	template<typename _T1, typename _T2>
	inline _T1 const angle_degrees(_T2 const & _p, _T2 const & _q)
	{
		return (_T1)(std::acos(_p.dot(_q) / (vector_norm2<_T1, _T2>(_p) * vector_norm2<_T1, _T2>(_q))) * (cnst::math::RIGHT_ANGLE * 2) / cnst::math::PI);
	}

	template <typename _T1, typename _T2>
	inline _T1 const vector_norm2(_T2 const & _p)
	{
		return (_T1)std::sqrt(_p.x * _p.x + _p.y * _p.y);
	}

	template <typename _T1, typename _T2>
	inline _T1 const vector_norm3(_T2 const & _p)
	{
		return (_T1)std::sqrt(_p.x * _p.x + _p.y * _p.y + _p.z * _p.z);
	}

	template <typename _T1, typename _T2, uint16_t D>
	inline _T1 const distance(_T2 const & _p, _T2 const & _q)
	{
		return vector_norm2<_T1, _T2>(_q - _p);
	}

	template <typename _T1, typename _T2>
	inline _T1 const cross(_T2 const & _p, _T2 const & _q)
	{
		return (_T1) (_p.x * _q.y - _p.y * _q.x);
	}

	template <typename _T>
	inline bool const intersection(_T & _t, _T const & _p, _T const & _q, _T const & _r, _T const & _s)
	{	
		_T const x = _r - _p;
		_T const d1 = _q - _p;
		_T const d2 = _s - _r;

		float cross_d1_d2 = cross<float, _T>(d1, d2);

		if (cross_d1_d2 < 1e-8f) 
			return false;

		float t1 = cross<float, _T>(x, d2) / cross_d1_d2;

		_t = (_T) (_p + d1 * t1);

		return true;
	}

private:

protected:
	Math() {}
};

template<typename _T >
class Quaternion
{
public:
	_T x, y, z, w;

	Quaternion() {}
	Quaternion(_T _x, _T _y, _T _z, _T _w)
		: x(_x), y(_y), z(_z), w(_w) {}

	~Quaternion() {}

	template<typename __T>
	static Quaternion const from_rodrigues_axis_angle(std::vector<__T> const & _vector)
	{
		cv::Mat R(3, 3, CV_64FC1);

		cv::Rodrigues(_vector, R);

		return Quaternion::from_rotation_matrix(R);
	}

	template<typename __T>
	static Quaternion const from_rotation_matrix(__T const & _R)
	{
		float m11 = _R.at<double>(0, 0); float m12 = _R.at<double>(0, 1); float m13 = _R.at<double>(0, 2);
		float m21 = _R.at<double>(1, 0); float m22 = _R.at<double>(1, 1); float m23 = _R.at<double>(1, 2);
		float m31 = _R.at<double>(2, 0); float m32 = _R.at<double>(2, 1); float m33 = _R.at<double>(2, 2);
		
		float w_ = std::sqrt(1 + m11 + m22 + m33) / 2;
		float x_ = std::sqrt(1 + m11 - m22 - m33) / 2;
		float y_ = std::sqrt(1 - m11 + m22 - m33) / 2;
		float z_ = std::sqrt(1 - m11 - m22 + m33) / 2;

		x_ = _copysign(x_, m32 - m23);
		y_ = _copysign(y_, m13 - m31);
		z_ = _copysign(z_, m21 - m12);

		return Quaternion(x_, y_, z_, w_);
	}

	template <typename __T>
	void const to_vector(std::vector<__T> & _vector) const
	{
		_vector = std::vector<_S>(4);

		_vector[0] = (__T) this->x;
		_vector[1] = (__T) this->y;
		_vector[2] = (__T) this->z;
		_vector[3] = (__T) this->w;
	}

	template <typename __T>
	void const to_pointer(__T * _pointer) const
	{
		_pointer[0] = (__T) this->x;
		_pointer[1] = (__T) this->y;
		_pointer[2] = (__T) this->z;
		_pointer[3] = (__T) this->w;
	}

private:
protected:
};


#endif // CV_PROCESSING_API_MATH_H_