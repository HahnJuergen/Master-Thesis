#include "pch.h"
#include "cv.h"

extern "C" 
{
	print_function m_debug;

	uint32_t m_image_width, m_image_height;

	cv::Mat m_camera_matrix;
	cv::Mat m_distortion_coefficients;

	float m_target_width;
	float m_target_height;

	bool m_is_initialized = false;

	CV_PROCESSING_API uint32_t initialize(uint32_t const _image_width, uint32_t const _image_height, float const _target_width, float const _target_height)
	{
		if (!m_is_initialized)
		{
			m_image_width = _image_width;
			m_image_height = _image_height;
			m_target_width = _target_width;
			m_target_height = _target_height;

			Processing::instance()->initialize_camera_related_matrices(m_camera_matrix, m_distortion_coefficients);
					
			m_is_initialized = true;

			return 0;
		}

		return 1;
	}

	CV_PROCESSING_API uint32_t process(byte * _image_data, float * _out_rvec, float * _out_tvec, uint32_t const _accuracy)
	{
		if (m_is_initialized)
		{
			cv::Mat mat(m_image_height, m_image_width, CV_8UC3, _image_data, m_image_width * CHANNELS);
	
			std::vector<cv::Point> points;
			std::vector<cv::Point2d> quad;
			std::vector<double> rotation_vector(3), translation_vector(3);

			Quaternion<double> rotation;

			Processing::instance()->perform_probabilistic_hough_transform(points, mat);
			Processing::instance()->approximate_polygon(quad, points);
			Processing::instance()->estimate_quadrilateral_pose(rotation, translation_vector, quad, m_camera_matrix, m_distortion_coefficients, m_target_width, m_target_height);
			
			return Processing::instance()->apply_data_to_out_datastructures(_out_rvec, _out_tvec, rotation, translation_vector, quad);
		}		
		
		return 2;
	}	

	CV_PROCESSING_API uint32_t set_debug_print(print_function _function_pointer)
	{
		m_debug = _function_pointer;

		return 0;
	}	

	CV_PROCESSING_API uint32_t is_lib_initialized()
	{
		return m_is_initialized ? 1 : 0;
	}
}