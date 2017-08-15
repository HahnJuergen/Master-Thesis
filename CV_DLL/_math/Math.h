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
		return (_T1)(std::acos(_p.dot(_q) / (vector_norm2<_T1, _T2>(_p) * vector_norm2<_T1, _T2>(_q))) * (RIGHT_ANGLE * 2) / PI);
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
	inline void const normalize2(_T & _vector)
	{	
		_vector /= vector_norm2<float, _T>(_vector);
	}

	template <typename _T>
	inline void const normalize3(_T & _vector)
	{
		_vector /= vector_norm3<float, _T>(_vector);
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
	Quaternion(_T _x, _T _y, _T _z, _T _w) : x(_x), y(_y), z(_z), w(_w) {}

	~Quaternion() {}

	template<typename __T>
	static Quaternion const from_rodrigues_axis_angle(std::vector<__T> const & _vector)
	{	
		cv::Point3f v(_vector[0], _vector[1], _vector[2]);
		
		float angle = Math::instance()->vector_norm3<float, cv::Point3f>(v) * 0.5f;
		float s = std::sin(angle);

		Math::instance()->normalize3<cv::Point3f>(v);

		Quaternion<__T> q(v.x * s, v.y * s, v.z * s, std::cos(angle));
		q.normalize();

		return q;
	}

	template<typename __T>
	static Quaternion const from_rotation_matrix(__T const & _R)
	{
		float const & r11 = _R.at<double>(0, 0); float const & r12 = _R.at<double>(0, 1); float const & r13 = _R.at<double>(0, 2);
		float const & r21 = _R.at<double>(1, 0); float const & r22 = _R.at<double>(1, 1); float const & r23 = _R.at<double>(1, 2);
		float const & r31 = _R.at<double>(2, 0); float const & r32 = _R.at<double>(2, 1); float const & r33 = _R.at<double>(2, 2);
		
		float w_ = std::sqrt(1 + r11 + r22 + r33) * 0.5f;
		float x_ = std::sqrt(1 + r11 - r22 - r33) * 0.5f;
		float y_ = std::sqrt(1 - r11 + r22 - r33) * 0.5f;
		float z_ = std::sqrt(1 - r11 - r22 + r33) * 0.5f;

		x_ = _copysign(x_, r32 - r23);
		y_ = _copysign(y_, r13 - r31);
		z_ = _copysign(z_, r21 - r12);

		return Quaternion(x_, y_, z_, w_);
	} 	

	template<typename __T> 
	__T const norm()
	{
		return (__T) std::sqrt(this->x * this->x + this->y * this->y + this->z * this->z + this->w * this->w);
	}

	template<typename __T>
	Quaternion<__T> & operator /= (__T _divisor)
	{
		this->x /= _divisor;
		this->y /= _divisor;
		this->z /= _divisor;
		this->w /= _divisor;

		return (* this);
	}

	template<typename __T>
	void operator >> (__T * _p) const
	{
		_p[0] = (__T) this->x;
		_p[1] = (__T) this->y;
		_p[2] = (__T) this->z;
		_p[3] = (__T) this->w;
	}

	template<typename __T>
	void operator >> (std::vector<__T> & _p) const
	{
		_p[0] = (__T) this->x;
		_p[1] = (__T) this->y;
		_p[2] = (__T) this->z;
		_p[3] = (__T) this->w;
	}

	void normalize()
	{
		(* this) /= this->norm<_T>();
	}

private:
protected:
};


#endif // CV_PROCESSING_API_MATH_H_