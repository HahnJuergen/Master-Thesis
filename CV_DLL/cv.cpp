#include "pch.h"
#include "cv.h"

extern "C" 
{
	print_function m_debug;

	uint32_t m_image_width, m_image_height;

	cv::Mat m_camera_matrix;
	cv::Mat m_distortion_coefficients;

	float m_target_width = 0.16f;
	float m_target_height = 0.1f;

	bool m_is_initialized = false;

	CV_PROCESSING_API uint32_t initialize(uint32_t const _width, uint32_t const _height)
	{
		if (!m_is_initialized)
		{
			m_image_width = _width;
			m_image_height = _height;

			Processing::instance()->initialize_camera_related_matrices(m_camera_matrix, m_distortion_coefficients);
					
			m_is_initialized = true;

			return 0;
		}

		return 1;
	}

	CV_PROCESSING_API uint32_t process(byte * _image_data, float * _out_corners, float * _out_rvec, float * _out_tvec, uint32_t const _accuracy)
	{
		if (m_is_initialized)
		{
			cv::Mat mat = cv::Mat(m_image_height, m_image_width, CV_8UC3, _image_data, m_image_width * 3);

			std::vector<cv::Point> points;
			std::vector<cv::Point2d> quad;

			Quaternion<double> rotation;
			std::vector<double> rotation_vector(3), translation_vector(3);

			Processing::instance()->perform_probabilistic_hough_transform(points, mat);
			Processing::instance()->approximate_polygon(quad, points, _accuracy);
			Processing::instance()->estimate_quadrilateral_pose(rotation, translation_vector, quad, m_camera_matrix, m_distortion_coefficients, m_target_width, m_target_height);
			
			return Processing::instance()->apply_data_to_out_datastructures(_out_corners, _out_rvec, _out_tvec, rotation, translation_vector, quad);
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