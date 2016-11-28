#!/bin/sh

LOG_DIR=$(dirname $0)
LOG_FILENAME=$(basename $0)
LOG_FILENAME=${LOG_FILENAME%.*}.log
LOG_FILE=${LOG_DIR}/${LOG_FILENAME}

Get_Running_Status ()
{
  local running_status=0
  local container_name=$1
  local container_status=0

  container_status=$(docker inspect "${container_name}" 2>&1)
  if [ $? = 0 ]; then
    container_status=$(echo $container_status | jq '.[0]["State"]["Status"]')
    if [ ${container_status//\"/} = "running" ]; then
      running_status=1
    else
      running_status=2
    fi
  fi

  return $running_status
}

Stop_Container ()
{
  local container_name=$1
  docker stop "${container_name}" &>>${LOG_FILE}
  return $?
}

Remove_Container ()
{
  local container_name=$1
  docker rm "${container_name}" &>>${LOG_FILE}
  return $?
}

Show_Running_Status ()
{
  local application_name=$1
  local container_name=$2
  local running_status

  Get_Running_Status "${container_name}"
  running_status=$?
  if [ $running_status = 1 ]; then
    echo ${application_name} is running.
  else
    echo ${application_name} is not running.
  fi

  return $running_status
}

Stop_Application ()
{
  local application_name=$1
  local container_name=$2
  local running_status
  local stop_result

  Get_Running_Status "${container_name}"
  running_status=$?
  if [ $running_status = 1 ]; then
    echo Stopping ${application_name} ...
    Stop_Container "${container_name}"
    stop_result=$?
    if [ $stop_result != 0 ]; then
      echo Failed to stop ${application_name}. See ${LOG_FILENAME} for detail.
    else
      echo ${application_name} is stopped.
    fi
    return $stop_result
  else
    echo ${application_name} is not running.
    return 2
  fi
}

Start_Application_As_Daemon ()
{
  local application_name=$1
  local image_tag=$2
  local container_name=$3
  local container_params=$4
  local running_status

  Get_Running_Status "${container_name}"
  running_status=$?

  if [ $running_status = 2 ]; then
    Remove_Container "${container_name}"
  fi
  if [ $running_status != 1 ]; then
    echo Starting ${application_name} ...
    eval "docker run -d --name \"\${container_name}\" \
        ${container_params} \
        ${image_tag} &>>${LOG_FILE}"
    if [ $? != 0 ]; then
      echo Failed to start ${application_name}. See ${LOG_FILENAME} for detail.
      return 1
    fi
    Wait_With_Animation 10
    Get_Running_Status "${container_name}"
    running_status=$?
    if [ $running_status = 1 ]; then
      echo ${application_name} is started. Before you can use ${application_name}, please wait at least another 30 seconds.
      return 0
    else
      echo Failed to start ${application_name}. See ${LOG_FILENAME} for detail.
      return 1
    fi
  else
    echo ${application_name} is already running.
    return 2
  fi
}

Wait_With_Animation()
{
  local wait_seconds=$1
  local idx
  local remainder
  for ((idx=0; idx<$wait_seconds; idx++)); do
    if [[ $idx > 0 ]]; then
      echo -en "\b"
    fi
    remainder=$(($idx % 4))
    if [[ $remainder = 0 ]]; then
      echo -en "-"
    elif [[ $remainder = 1 ]]; then
      echo -en "\\"
    elif [[ $remainder = 2 ]]; then
      echo -en "|"
    elif [[ $remainder = 3 ]]; then
      echo -en "/"
    fi
    sleep 1
  done
  if [[ $wait_seconds > 0 ]]; then
    echo -en "\b"
  fi
}
