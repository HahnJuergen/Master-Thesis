#include "pch.h"
#include "cv.h"

extern "C" 
{
	print_function m_debug;

	int m_image_width, m_image_height;

	cv::Mat m_camera_matrix;
	cv::Mat m_distortion_coefficients;

	float m_target_width = 0.16f;
	float m_target_height = 0.1f;

	bool m_is_initialized = false;

	CV_PROCESSING_API int initialize(int const _width, int const _height)
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

	CV_PROCESSING_API int process(unsigned char * _image_data, float * _corners, float * _out_rvec, float * _out_tvec)
	{
		if (m_is_initialized)
		{
			cv::Mat mat = cv::Mat(m_image_height, m_image_width, CV_8UC3, _image_data, m_image_width * 3);

			std::vector<cv::Point> points;

			std::vector<cv::Point2d> quad;

			std::vector<double> rotation_vector(3), translation_vector(3);

			Processing::instance()->perform_probabilistic_hough_transform(points, mat);
			Processing::instance()->approximate_quadrilateral(quad, points);
			Processing::instance()->estimate_quadrilateral_pose(rotation_vector, translation_vector, quad, m_camera_matrix, m_distortion_coefficients, m_target_width, m_target_height);

			if (quad.size() == 4)
			{
				Utility::draw_polygon(mat, quad, Utility::GREEN);
				
				_image_data = mat.data;

				return Processing::instance()->apply_data_to_out_datastructures(_corners, _out_rvec, _out_tvec, rotation_vector, translation_vector, quad);
			}
			else
			{
				_image_data = mat.data;

				return 1;
			}
		}		
		
		return 2;
	}	

	CV_PROCESSING_API int set_debug_print(print_function _function_pointer)
	{
		m_debug = _function_pointer;

		return 0;
	}	

	CV_PROCESSING_API int is_lib_initialized()
	{
		return m_is_initialized ? 1 : 0;
	}
}