# Minimum required version of CMake
cmake_minimum_required(VERSION 3.10)

# Project name and language
project(MyProject C)

# Specify the C standard
set(CMAKE_C_STANDARD 99)
set(CMAKE_C_STANDARD_REQUIRED True)

# Add the src directory to the list of directories to search for source files
include_directories(src)

# Collect all the source files in the src directory
file(GLOB SOURCES "src/*.c")

# Add an executable with the collected source files
add_executable(MyProject ${SOURCES})
