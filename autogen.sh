
error () {
	echo "Error: $1" 1>&2
	exit 1
}

check_autotool_version () {
	which $1 &>/dev/null || {
		error "$1 is not installed, and is required to configure $PACKAGE"
	}

	version=$($1 --version | head -n 1 | cut -f4 -d' ')
	major=$(echo $version | cut -f1 -d.)
	minor=$(echo $version | cut -f2 -d.)
	rev=$(echo $version | cut -f3 -d.)
	major_check=$(echo $2 | cut -f1 -d.)
	minor_check=$(echo $2 | cut -f2 -d.)
	rev_check=$(echo $2 | cut -f3 -d.)

	if [ $major -lt $major_check ]; then
		do_bail=yes
	elif [ $minor -lt $minor_check ] && [ $major = $major_check ]; then
		do_bail=yes
	elif [ x"$rev_check" != x"" ] && [ $rev -lt $rev_check ] && [ $minor = $minor_check ] && [ $major = $major_check ]; then
		do_bail=yes
	fi

	if [ x"$do_bail" = x"yes" ]; then
		error "$1 version $2 or better is required to configure $PROJECT"
	fi
}


srcdir=`dirname $0`
test -z "$srcdir" && srcdir=.

ORIGDIR=`pwd`
cd $srcdir
PROJECT=gbrainy
TEST_TYPE=-f
FILE=src/Makefile.am

DIE=0

(autoconf --version) < /dev/null > /dev/null 2>&1 || {
	echo
	echo "You must have autoconf installed to compile $PROJECT."
	echo "Download the appropriate package for your distribution,"
	echo "or get the source tarball at ftp://ftp.gnu.org/pub/gnu/"
	DIE=1
}

AUTOMAKE=automake
ACLOCAL=aclocal

check_autotool_version $ACLOCAL 1.10
check_autotool_version $AUTOMAKE 1.10

(grep "^AM_PROG_LIBTOOL" configure.ac >/dev/null) && {
  (libtool --version) < /dev/null > /dev/null 2>&1 || {
    echo
    echo "**Error**: You must have \`libtool' installed to compile $PROJECT."
    echo "Get ftp://ftp.gnu.org/pub/gnu/libtool-1.2d.tar.gz"
    echo "(or a newer version if it is available)"
    DIE=1
  }
}

grep "^AM_GLIB_GNU_GETTEXT" configure.ac >/dev/null && {
  grep "sed.*POTFILES" $srcdir/configure.ac >/dev/null || \
  (glib-gettextize --version) < /dev/null > /dev/null 2>&1 || {
    echo
    echo "**Error**: You must have \`glib-devel' (or glib2-devel) installed to compile $PROJECT."
    DIE=1
  }
}

(which yelp-build)|| {
	echo "**Error**: You must have yelp-tools installed to compile $PROJECT."
	DIE=1
}

if test "$DIE" -eq 1; then
	exit 1
fi

test $TEST_TYPE $FILE || {
	echo "You must run this script in the top-level $PROJECT directory"
	exit 1
}

if test -z "$*"; then
	echo "I am going to run ./configure with no arguments - if you wish "
        echo "to pass any to it, please specify them on the $0 command line."
fi

case $CC in
*xlc | *xlc\ * | *lcc | *lcc\ *) am_opt=--include-deps;;
esac

for coin in `find $srcdir -name configure.ac -print`
do 
  dr=`dirname $coin`
  if test -f $dr/NO-AUTO-GEN; then
    echo skipping $dr -- flagged as no auto-gen
  else
    echo processing $dr
    macrodirs=`sed -n -e 's,AM_ACLOCAL_INCLUDE(\(.*\)),\1,gp' < $coin`
    ( cd $dr
      aclocalinclude="$ACLOCAL_FLAGS"
      for k in $macrodirs; do
  	if test -d $k; then
          aclocalinclude="$aclocalinclude -I $k"
  	##else 
	##  echo "**Warning**: No such directory \`$k'.  Ignored."
        fi
      done
      if grep "^AM_GNU_GETTEXT" configure.ac >/dev/null; then
	if grep "sed.*POTFILES" configure.ac >/dev/null; then
	  : do nothing -- we still have an old unmodified configure.ac
	else
	  echo "Creating $dr/aclocal.m4 ..."
	  test -r $dr/aclocal.m4 || touch $dr/aclocal.m4
	  echo "Running gettextize...  Ignore non-fatal messages."
	  echo "no" | gettextize --force --copy
	  echo "Making $dr/aclocal.m4 writable ..."
	  test -r $dr/aclocal.m4 && chmod u+w $dr/aclocal.m4
        fi
      fi
      if grep "^AM_GNOME_GETTEXT" configure.ac >/dev/null; then
	echo "Creating $dr/aclocal.m4 ..."
	test -r $dr/aclocal.m4 || touch $dr/aclocal.m4
	echo "Running gettextize...  Ignore non-fatal messages."
	echo "no" | gettextize --force --copy
	echo "Making $dr/aclocal.m4 writable ..."
	test -r $dr/aclocal.m4 && chmod u+w $dr/aclocal.m4
      fi
      if grep "^AM_GLIB_GNU_GETTEXT" configure.ac >/dev/null; then
	echo "Creating $dr/aclocal.m4 ..."
	test -r $dr/aclocal.m4 || touch $dr/aclocal.m4
	echo "Running gettextize...  Ignore non-fatal messages."
	echo "no" | glib-gettextize --force --copy
	echo "Making $dr/aclocal.m4 writable ..."
	test -r $dr/aclocal.m4 && chmod u+w $dr/aclocal.m4
      fi
      if grep "^AC_PROG_INTLTOOL" configure.ac >/dev/null ||
         grep "^IT_PROG_INTLTOOL" configure.ac >/dev/null; then
        echo "Running intltoolize..."
        intltoolize --copy --force --automake
      fi
      if grep "^AM_PROG_LIBTOOL" configure.ac >/dev/null; then
	echo "Running libtoolize..."
	libtoolize --force --copy
      fi
      echo "Running $ACLOCAL $aclocalinclude ..."
      $ACLOCAL $aclocalinclude
      if grep "^AM_CONFIG_HEADER" configure.ac >/dev/null; then
	echo "Running autoheader..."
	autoheader
      fi
      echo "Running $AUTOMAKE --gnu $am_opt ..."
      $AUTOMAKE --add-missing --gnu $am_opt
      echo "Running autoconf ..."
      autoconf
    )
  fi
done

conf_flags="--enable-maintainer-mode --enable-compile-warnings" #--enable-iso-c

cd "$ORIGDIR"

if test x$NOCONFIGURE = x; then
  echo Running $srcdir/configure $conf_flags "$@" ...
  $srcdir/configure $conf_flags "$@" \
  && echo Now type \`make\' to compile $PROJECT  || exit 1
else
  echo Skipping configure process.
fi
