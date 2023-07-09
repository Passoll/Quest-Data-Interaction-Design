import h5py
import pathlib


def to_tuple_list(numpy_list):
    """Data imported with h5py is usually given as numpy array.
    This function transforms numpy arrays to tuple.

    Args:
        numpy_list (numpy.ndarray): lists of 3D positions as numpy array.

    Returns:
        list: lists of 3D positions as tuple.
    """
    return [tuple(entry) for entry in numpy_list]


def import_all_from_hdf5(
    file_path: pathlib.Path,
    group: str = "originals",
) -> list:
    """Imports all stored ball data from HDF5 file specified in given
    path argument from specified group.

    Args:
        group (str, optional): Nested group within HDF5 file.
        Defaults to "originals".
        file_path (pathlib.Path, optional): Path object specifying
        file location. Defaults to None.
    """

    # "r" specifies only read permissions
    file = h5py.File(file_path, "r")

    trajectory_data = []

    for index in list(file[group].keys()):
        launch_parameter = tuple(file[group][index]["launch_param"])
        time_stamps = list(file[group][index]["time_stamps"])
        positions = to_tuple_list(list(file[group][index]["positions"]))
        velocities = to_tuple_list(list(file[group][index]["velocities"]))

        trajectory_data.append([launch_parameter, time_stamps, positions, velocities])

    file.close()

    return trajectory_data


if __name__ == "__main__":
    path = pathlib.Path("./MN5008_training_data/MN5008_grid_data_all.hdf5")
    trajectory_data = import_all_from_hdf5(file_path=path)
