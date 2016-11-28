#!/bin/sh

. $(dirname $0)/docker-common.inc

APPLICATION_NAME=Jenkins

JENKINS_IMAGE_TAG=jenkins:2.7.1
JENKINS_NAME=kddi-jenkins
JENKINS_DATA_DIR=/srv/jenkins
JENKINS_PORT=8877
JENKINS_SLAVE_PORT=58877
JENKINS_PARAMS=" \
    -e JAVA_OPTS=\"-Djava.awt.headless=true\" \
    -e JENKINS_OPTS=\"--httpPort=${JENKINS_PORT}\" \
    -e JENKINS_SLAVE_AGENT_PORT=${JENKINS_SLAVE_PORT} \
    -p ${JENKINS_PORT}:${JENKINS_PORT} \
    -p ${JENKINS_SLAVE_PORT}:${JENKINS_SLAVE_PORT} \
    -v ${JENKINS_DATA_DIR}:/var/jenkins_home"

status ()
{
  Show_Running_Status "${APPLICATION_NAME}" "${JENKINS_NAME}"
  RETVAL=$?
  return $RETVAL
}

stop()
{
  Stop_Application "${APPLICATION_NAME}" "${JENKINS_NAME}"
  RETVAL=$?
  return $RETVAL
}

start ()
{
  Start_Application_As_Daemon "${APPLICATION_NAME}" "${JENKINS_IMAGE_TAG}" "${JENKINS_NAME}" "${JENKINS_PARAMS}"
  RETVAL=$?
  return $RETVAL
}

RETVAL=0

case "$1" in
  start)
        start
        ;;
  stop)
        stop
        ;;
  restart)
        stop
        start
        RETVAL=$?
        ;;
  status)
        status
        RETVAL=$?
        ;;
  *)
        echo $"Usage: $0 {start|stop|restart|status}"
        exit 9
esac

exit $RETVAL
